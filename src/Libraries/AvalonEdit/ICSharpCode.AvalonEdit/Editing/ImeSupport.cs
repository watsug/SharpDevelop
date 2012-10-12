﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace ICSharpCode.AvalonEdit.Editing
{
	class ImeSupport
	{
		readonly TextArea textArea;
		IntPtr currentContext;
		HwndSource hwndSource;
		
		public ImeSupport(TextArea textArea)
		{
			if (textArea == null)
				throw new ArgumentNullException("textArea");
			this.textArea = textArea;
			InputMethod.SetIsInputMethodSuspended(this.textArea, true);
			textArea.OptionChanged += TextAreaOptionChanged;
			currentContext = IntPtr.Zero;
		}

		void TextAreaOptionChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "EnableImeSupport" && textArea.IsKeyboardFocused) {
				CreateContext();
			}
		}

		void ClearContext()
		{
			if (hwndSource != null) {
				ImeNativeWrapper.ReleaseContext(hwndSource, currentContext);
				currentContext = IntPtr.Zero;
				hwndSource.RemoveHook(WndProc);
				hwndSource = null;
			}
		}
		
		public void OnGotFocus(KeyboardFocusChangedEventArgs e)
		{
			CreateContext();
		}

		void CreateContext()
		{
			ClearContext(); // clear old context if necessary
			if (!textArea.Options.EnableImeSupport)
				return;
			hwndSource = (HwndSource)PresentationSource.FromVisual(this.textArea);
			if (hwndSource != null) {
				currentContext = ImeNativeWrapper.GetContext(hwndSource);
//				ImeNativeWrapper.SetCompositionFont(hwndSource, currentContext, textArea);
				hwndSource.AddHook(WndProc);
				// UpdateCompositionWindow() will be called by the caret becoming visible
			}
		}
		
		public void OnLostFocus(KeyboardFocusChangedEventArgs e)
		{
			if (e.OldFocus == textArea && currentContext != IntPtr.Zero)
				ImeNativeWrapper.NotifyIme(currentContext);
			ClearContext();
		}
		
		IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			switch (msg) {
				case ImeNativeWrapper.WM_INPUTLANGCHANGE:
					// Don't mark the message as handled; other windows
					// might want to handle it as well.
					ClearContext();
					CreateContext();
					break;
				case ImeNativeWrapper.WM_IME_COMPOSITION:
					UpdateCompositionWindow();
					break;
			}
			return IntPtr.Zero;
		}
		
		public void UpdateCompositionWindow()
		{
			if (currentContext != IntPtr.Zero) {
				ImeNativeWrapper.SetCompositionWindow(hwndSource, currentContext, textArea);
			}
		}
	}
}