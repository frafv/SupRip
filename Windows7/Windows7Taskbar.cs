// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Windows7.DesktopIntegration.Interop;

namespace Windows7.DesktopIntegration
{
	/// <summary>
	/// The primary coordinator of the Windows 7 taskbar-related activities.
	/// For additional functionality, see the <see cref="JumpListManager"/>
	/// and <see cref="ThumbButtonManager"/> classes.
	/// </summary>
	public static class Windows7Taskbar
	{
		#region Infrastructure

		/// <summary>
		/// The WM_TaskbarButtonCreated message number.
		/// </summary>
		public static uint TaskbarButtonCreatedMessage
		{
			get { return UnsafeNativeMethods.WM_TaskbarButtonCreated; }
		}

		private static Lazy<ITaskbarList3> _taskbarList = new Lazy<ITaskbarList3>(() =>
			{
				ITaskbarList3 taskbarList;
				if (Supported)
				{
					taskbarList = (ITaskbarList3)new CTaskbarList();
					taskbarList.HrInit();
				}
				else
				{
					try
					{
						taskbarList = new EmptyTaskList(new CTaskbarList());
					}
					catch
					{
						taskbarList = new EmptyTaskList();
					}
				}
				return taskbarList;
			});

		internal static ITaskbarList3 TaskbarList
		{
			get
			{
				return _taskbarList.Value;
			}
		}

		private static IPropertyStore InternalGetWindowPropertyStore( IntPtr hwnd )
		{
			IPropertyStore propStore;
			int rc = UnsafeNativeMethods.SHGetPropertyStoreForWindow(
					hwnd,
					ref SafeNativeMethods.IID_IPropertyStore,
					out propStore );
			if ( rc != 0 )
				throw Marshal.GetExceptionForHR( rc );
			return propStore;
		}

		private static void InternalEnableCustomWindowPreview( IntPtr hwnd, bool enable )
		{
			IntPtr t = Marshal.AllocHGlobal( 4 );
			Marshal.WriteInt32( t, enable ? 1 : 0 );

			try
			{
				int rc;
				rc = UnsafeNativeMethods.DwmSetWindowAttribute(
						hwnd, SafeNativeMethods.DWMWA_HAS_ICONIC_BITMAP, t, 4 );
				if ( rc != 0 )
					throw Marshal.GetExceptionForHR( rc );

				rc = UnsafeNativeMethods.DwmSetWindowAttribute(
						hwnd, SafeNativeMethods.DWMWA_FORCE_ICONIC_REPRESENTATION, t, 4 );
				if ( rc != 0 )
					throw Marshal.GetExceptionForHR( rc );
			}
			finally
			{
				Marshal.FreeHGlobal( t );
			}
		}

		#endregion

		#region Application Id

		/// <summary>
		/// Gets a window's application id by its window handle.
		/// </summary>
		/// <param name="hwnd">The window handle.</param>
		/// <returns>The application id of that window.</returns>
		public static string GetWindowAppId( IntPtr hwnd )
		{
			if ( !Supported )
				throw new PlatformNotSupportedException();

			IPropertyStore propStore = InternalGetWindowPropertyStore( hwnd );

			PropVariant pv;
			propStore.GetValue( ref PropertyKey.PKEY_AppUserModel_ID, out pv );

			Marshal.ReleaseComObject( propStore );

			return pv.GetValue();
		}
		/// <summary>
		/// Sets the window's application id by its window handle.
		/// </summary>
		/// <param name="hwnd">The window handle.</param>
		/// <param name="appId">The application id.</param>
		public static void SetWindowAppId( IntPtr hwnd, string appId )
		{
			if ( !Supported )
				throw new PlatformNotSupportedException();

			IPropertyStore propStore = InternalGetWindowPropertyStore( hwnd );

			PropVariant pv = new PropVariant();
			pv.SetValue( appId );
			propStore.SetValue( ref PropertyKey.PKEY_AppUserModel_ID, ref pv );

			Marshal.ReleaseComObject( propStore );
		}

		/// <summary>
		/// Sets the current process' explicit application user model id.
		/// </summary>
		/// <param name="appId">The application id.</param>
		public static void SetCurrentProcessAppId( string appId )
		{
			if ( !Supported )
				throw new PlatformNotSupportedException();

			UnsafeNativeMethods.SetCurrentProcessExplicitAppUserModelID( appId );
		}
		/// <summary>
		/// Gets the current process' explicit application user model id.
		/// </summary>
		/// <returns>The application id.</returns>
		public static string GetCurrentProcessAppId()
		{
			if ( !Supported )
				throw new PlatformNotSupportedException();

			string appId;
			UnsafeNativeMethods.GetCurrentProcessExplicitAppUserModelID( out appId );
			return appId;
		}

		#endregion

		#region DWM Iconic Thumbnail and Peek Bitmap

		/// <summary>
		/// Indicates that the specified window requests the DWM
		/// to demand live preview (thumbnail and peek) mode when necessary
		/// instead of relying on default preview.
		/// </summary>
		/// <param name="hwnd">The window handle.</param>
		public static void EnableCustomWindowPreview( IntPtr hwnd )
		{
			if ( !SupportedVista )
				throw new PlatformNotSupportedException();

			InternalEnableCustomWindowPreview( hwnd, true );
		}
		/// <summary>
		/// Indicates that the specified window does not require the DWM
		/// to demand live preview (thumbnail and peek) mode when necessary,
		/// i.e. this window relies on default preview.
		/// </summary>
		/// <param name="hwnd">The window handle.</param>
		public static void DisableCustomWindowPreview( IntPtr hwnd )
		{
			if ( !SupportedVista )
				throw new PlatformNotSupportedException();

			InternalEnableCustomWindowPreview( hwnd, false );
		}
		/// <summary>
		/// Sets the specified iconic thumbnail for the specified window.
		/// This is typically done in response to a DWM message.
		/// </summary>
		/// <param name="hwnd">The window handle.</param>
		/// <param name="bitmap">The thumbnail bitmap.</param>
		public static void SetIconicThumbnail( IntPtr hwnd, Bitmap bitmap )
		{
			if ( !Supported )
				throw new PlatformNotSupportedException();

			int rc = UnsafeNativeMethods.DwmSetIconicThumbnail(
					hwnd,
					bitmap.GetHbitmap(),
					SafeNativeMethods.DWM_SIT_DISPLAYFRAME );
			if ( rc != 0 )
				throw Marshal.GetExceptionForHR( rc );
		}
		/// <summary>
		/// Sets the specified peek (live preview) bitmap for the specified
		/// window.  This is typically done in response to a DWM message.
		/// </summary>
		/// <param name="hwnd">The window handle.</param>
		/// <param name="bitmap">The thumbnail bitmap.</param>
		/// <param name="displayFrame">Whether to display a standard window
		/// frame around the bitmap.</param>
		public static void SetPeekBitmap( IntPtr hwnd, Bitmap bitmap, bool displayFrame )
		{
			if ( !Supported )
				throw new PlatformNotSupportedException();

			int rc = UnsafeNativeMethods.DwmSetIconicLivePreviewBitmap(
					hwnd,
					bitmap.GetHbitmap(),
					IntPtr.Zero,
					displayFrame ? SafeNativeMethods.DWM_SIT_DISPLAYFRAME : (uint)0 );
			if ( rc != 0 )
				throw Marshal.GetExceptionForHR( rc );
		}
		/// <summary>
		/// Sets the specified peek (live preview) bitmap for the specified
		/// window.  This is typically done in response to a DWM message.
		/// </summary>
		/// <param name="hwnd">The window handle.</param>
		/// <param name="bitmap">The thumbnail bitmap.</param>
		/// <param name="offset">The client area offset at which to display
		/// the specified bitmap.  The rest of the parent window will be
		/// displayed as "remembered" by the DWM.</param>
		/// <param name="displayFrame">Whether to display a standard window
		/// frame around the bitmap.</param>
		public static void SetPeekBitmap( IntPtr hwnd, Bitmap bitmap, Point offset, bool displayFrame )
		{
			if ( !Supported )
				throw new PlatformNotSupportedException();

			var nativePoint = new /*VistaBridgeInterop.*/SafeNativeMethods.POINT( offset.X, offset.Y );
			int rc = UnsafeNativeMethods.DwmSetIconicLivePreviewBitmap(
					hwnd,
					bitmap.GetHbitmap(),
					ref nativePoint,
					displayFrame ? SafeNativeMethods.DWM_SIT_DISPLAYFRAME : (uint)0 );
			if ( rc != 0 )
				throw Marshal.GetExceptionForHR( rc );
		}

		#endregion

		#region Taskbar Overlay Icon

		/// <summary>
		/// Draws the specified overlay icon on the specified window's
		/// taskbar button.
		/// </summary>
		/// <param name="hwnd">The window handle.</param>
		/// <param name="icon">The overlay icon.</param>
		/// <param name="description">The overlay icon description.</param>
		public static void SetTaskbarOverlayIcon( IntPtr hwnd, Icon icon, string description )
		{
			if ( !Supported )
				throw new PlatformNotSupportedException();

			TaskbarList.SetOverlayIcon(
					hwnd,
					icon == null ? IntPtr.Zero : icon.Handle,
					description );
		}

		#endregion

		#region Taskbar Progress Bar

		/// <summary>
		/// Represents the thumbnail progress bar state.
		/// </summary>
		public enum ThumbnailProgressState
		{
			/// <summary>
			/// No progress is displayed.
			/// </summary>
			NoProgress = 0,
			/// <summary>
			/// The progress is indeterminate (marquee).
			/// </summary>
			Indeterminate = 0x1,
			/// <summary>
			/// Normal progress is displayed.
			/// </summary>
			Normal = 0x2,
			/// <summary>
			/// An error occurred (red).
			/// </summary>
			Error = 0x4,
			/// <summary>
			/// The operation is paused (yellow).
			/// </summary>
			Paused = 0x8
		}

		/// <summary>
		/// Sets the progress state of the specified window's
		/// taskbar button.
		/// </summary>
		/// <param name="hwnd">The window handle.</param>
		/// <param name="state">The progress state.</param>
		public static void SetProgressState( IntPtr hwnd,
				ThumbnailProgressState state )
		{
			TaskbarList.SetProgressState( hwnd, (TBPFLAG)state );
		}
		/// <summary>
		/// Sets the progress value of the specified window's
		/// taskbar button.
		/// </summary>
		/// <param name="hwnd">The window handle.</param>
		/// <param name="current">The current value.</param>
		/// <param name="maximum">The maximum value.</param>
		public static void SetProgressValue( IntPtr hwnd,
				ulong current, ulong maximum )
		{
			TaskbarList.SetProgressValue( hwnd, current, maximum );
		}

		#endregion

		#region Taskbar Thumbnails

		/// <summary>
		/// Specifies that only a portion of the window's client area
		/// should be used in the window's thumbnail.
		/// </summary>
		/// <param name="hwnd">The window.</param>
		/// <param name="clipRect">The rectangle that specifies the clipped region.</param>
		public static void SetThumbnailClip( IntPtr hwnd, Rectangle clipRect )
		{
			if ( !Supported )
				throw new PlatformNotSupportedException();

			RECT rect = new RECT( clipRect.Left, clipRect.Top, clipRect.Right, clipRect.Bottom );
			TaskbarList.SetThumbnailClip( hwnd, ref rect );
		}

		/// <summary>
		/// Sets the specified window's thumbnail tooltip.
		/// </summary>
		/// <param name="hwnd">The window.</param>
		/// <param name="tooltip">The tooltip text.</param>
		public static void SetThumbnailTooltip( IntPtr hwnd, string tooltip )
		{
			if ( !Supported )
				throw new PlatformNotSupportedException();

			TaskbarList.SetThumbnailTooltip( hwnd, tooltip );
		}

		#endregion

		#region Miscellaneous

		/// <summary>
		/// Specifies that the taskbar- and DWM-related windows messages should
		/// pass through the Windows UIPI mechanism even if the process is
		/// running elevated.  Calling this method is not required unless the
		/// process is running elevated.
		/// </summary>
		public static void AllowTaskbarWindowMessagesThroughUIPI()
		{
			UnsafeNativeMethods.ChangeWindowMessageFilter(
					UnsafeNativeMethods.WM_TaskbarButtonCreated,
					SafeNativeMethods.MSGFLT_ADD );
			UnsafeNativeMethods.ChangeWindowMessageFilter(
					SafeNativeMethods.WM_DWMSENDICONICTHUMBNAIL,
					SafeNativeMethods.MSGFLT_ADD );
			UnsafeNativeMethods.ChangeWindowMessageFilter(
					SafeNativeMethods.WM_DWMSENDICONICLIVEPREVIEWBITMAP,
					SafeNativeMethods.MSGFLT_ADD );
			UnsafeNativeMethods.ChangeWindowMessageFilter(
					SafeNativeMethods.WM_COMMAND,
					SafeNativeMethods.MSGFLT_ADD );
			UnsafeNativeMethods.ChangeWindowMessageFilter(
					SafeNativeMethods.WM_SYSCOMMAND,
					SafeNativeMethods.MSGFLT_ADD );
			UnsafeNativeMethods.ChangeWindowMessageFilter(
					SafeNativeMethods.WM_ACTIVATE,
					SafeNativeMethods.MSGFLT_ADD );
		}


		public static bool Supported
		{
			get
			{
				if ( System.Environment.OSVersion.Version.Major == 6 )
				{
					return (System.Environment.OSVersion.Version.Minor >= 1);
				}
				else
				{
					return (System.Environment.OSVersion.Version.Major > 6);
				}
			}
		}

		public static bool SupportedVista
		{
			get
			{
				return (System.Environment.OSVersion.Version.Major >= 6);
			}
		}

		private class EmptyTaskList : ITaskbarList3
		{
			private ITaskbarList2 _taskList2;
			private ITaskbarList _taskList;
			public EmptyTaskList( CTaskbarList taskList )
			{
				_taskList = taskList as ITaskbarList;
				_taskList2 = taskList as ITaskbarList2;
			}

			public EmptyTaskList()
			{
				_taskList = null;
				_taskList2 = null;
			}

			#region ITaskbarList3 Members

			void ITaskbarList3.HrInit()
			{
				if ( _taskList != null ) _taskList.HrInit();
			}

			void ITaskbarList3.AddTab( IntPtr hwnd )
			{
				if ( _taskList != null ) _taskList.AddTab( hwnd );
			}

			void ITaskbarList3.DeleteTab( IntPtr hwnd )
			{
				if ( _taskList != null ) _taskList.DeleteTab( hwnd );
			}

			void ITaskbarList3.ActivateTab( IntPtr hwnd )
			{
				if ( _taskList != null ) _taskList.ActivateTab( hwnd );
			}

			void ITaskbarList3.SetActiveAlt( IntPtr hwnd )
			{
				if ( _taskList != null ) _taskList.SetActiveAlt( hwnd );
			}

			void ITaskbarList3.MarkFullscreenWindow( IntPtr hwnd, bool fFullscreen )
			{
				if ( _taskList2 != null ) _taskList2.MarkFullscreenWindow( hwnd, fFullscreen );
			}

			void ITaskbarList3.SetProgressValue( IntPtr hwnd, ulong ullCompleted, ulong ullTotal ) {}
			void ITaskbarList3.SetProgressState( IntPtr hwnd, TBPFLAG tbpFlags ) {}
			void ITaskbarList3.RegisterTab( IntPtr hwndTab, IntPtr hwndMDI ) {}
			void ITaskbarList3.UnregisterTab( IntPtr hwndTab ) {}
			void ITaskbarList3.SetTabOrder( IntPtr hwndTab, IntPtr hwndInsertBefore ) {}
			void ITaskbarList3.SetTabActive( IntPtr hwndTab, IntPtr hwndMDI, TBATFLAG tbatFlags ) {}
			void ITaskbarList3.ThumbBarAddButtons( IntPtr hwnd, uint cButtons, THUMBBUTTON[] pButtons ) {}
			void ITaskbarList3.ThumbBarUpdateButtons( IntPtr hwnd, uint cButtons, THUMBBUTTON[] pButtons ) {}
			void ITaskbarList3.ThumbBarSetImageList( IntPtr hwnd, IntPtr himl ) {}
			void ITaskbarList3.SetOverlayIcon( IntPtr hwnd, IntPtr hIcon, string pszDescription ) {}
			void ITaskbarList3.SetThumbnailTooltip( IntPtr hwnd, string pszTip ) {}
			void ITaskbarList3.SetThumbnailClip( IntPtr hwnd, ref RECT prcClip ) {}

			#endregion
		}

		#endregion
	}
}