// Copyright (c) Microsoft Corporation.  All rights reserved.

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Windows7.DesktopIntegration.WindowsForms
{
	/// <summary>
	/// Contains extension methods for easier Windows Forms interoperability.
	/// </summary>
	public static class WindowsFormsExtensions
	{
		/// <summary>
		/// Sets this form's application id.
		/// </summary>
		/// <param name="form">The form.</param>
		/// <param name="appId">The application id.</param>
		public static void SetAppId( this Form form, string appId )
		{
			if ( Windows7Taskbar.Supported )
				Windows7Taskbar.SetWindowAppId( form.Handle, appId );
		}
		/// <summary>
		/// Gets this form's application id.
		/// </summary>
		/// <param name="form">The form.</param>
		/// <returns>The form's application id.</returns>
		public static string GetAppId( this Form form )
		{
			if ( !Windows7Taskbar.Supported )
				return null;

			return Windows7Taskbar.GetWindowAppId( form.Handle );
		}
		/// <summary>
		/// Enables custom window preview on this form, meaning that
		/// the DWM will send messages when a live thumbnail preview
		/// or a live peek will be necessary.
		/// </summary>
		/// <param name="form">The form.</param>
		public static void EnableCustomWindowPreview( this Form form )
		{
			if ( Windows7Taskbar.SupportedVista )
				Windows7Taskbar.EnableCustomWindowPreview( form.Handle );
		}
		/// <summary>
		/// Disables custom window preview on this form, meaning that
		/// the DWM will not send messages when a live thumbnail preview
		/// or a live peek will be necessary, but instead rely on its
		/// default rendering.
		/// </summary>
		/// <param name="form">The form.</param>
		public static void DisableCustomWindowPreview( this Form form )
		{
			if ( Windows7Taskbar.SupportedVista )
				Windows7Taskbar.DisableCustomWindowPreview( form.Handle );
		}
		/// <summary>
		/// Sets this form's iconic thumbnail to the specified bitmap.
		/// </summary>
		/// <param name="form">The form.</param>
		/// <param name="bitmap">The bitmap.</param>
		public static void SetIconicThumbnail( this Form form, Bitmap bitmap )
		{
			if ( Windows7Taskbar.Supported )
				Windows7Taskbar.SetIconicThumbnail( form.Handle, bitmap );
		}
		/// <summary>
		/// Sets this form's peek (live preview) bitmap.
		/// </summary>
		/// <param name="form">The form.</param>
		/// <param name="bitmap">The bitmap.</param>
		/// <param name="displayFrame">Whether to display a standard window
		/// frame around the bitmap.</param>
		public static void SetPeekBitmap( this Form form, Bitmap bitmap, bool displayFrame )
		{
			if ((form = FindFormInTaskbar(form)) == null) return;
			if (Windows7Taskbar.Supported)
				Windows7Taskbar.SetPeekBitmap( form.Handle, bitmap, displayFrame );
		}
		/// <summary>
		/// Draws the specified overlay icon over this form's taskbar button.
		/// </summary>
		/// <param name="form">The form.</param>
		/// <param name="icon">The overlay icon.</param>
		/// <param name="description">The overlay icon's description.</param>
		public static void SetTaskbarOverlayIcon( this Form form, Icon icon, string description )
		{
			if ( Windows7Taskbar.Supported )
				Windows7Taskbar.SetTaskbarOverlayIcon( form.Handle, icon, description );
		}
		private static Form FindFormInTaskbar( Form form )
		{
			if ( form != null && form.ShowInTaskbar ) return form;
			foreach ( Form app in Application.OpenForms )
				if ( app.ShowInTaskbar )
					return app;
			return form;
		}
		/// <summary>
		/// Sets the progress bar in the containing form's taskbar button
		/// to this progress bar's progress.
		/// </summary>
		/// <param name="progressBar">The progress bar.</param>
		public static void SetTaskbarProgress( this ProgressBar progressBar )
		{
			if ( !Windows7Taskbar.Supported ) return;

			Form form = (Form)progressBar.TopLevelControl;
			if ( (form = FindFormInTaskbar( form )) == null ) return;

			//Approximation:
			ulong maximum = (ulong)(progressBar.Maximum - progressBar.Minimum);
			ulong progress = (ulong)(progressBar.Value - progressBar.Minimum);

			Windows7Taskbar.SetProgressState(
					form.Handle, Windows7Taskbar.ThumbnailProgressState.Normal );
			Windows7Taskbar.SetProgressValue(
					form.Handle, progress, maximum );
		}
		/// <summary>
		/// Hides the progress bar in the containing form's taskbar button.
		/// </summary>
		/// <param name="progressBar">The progress bar.</param>
		public static void ResetTaskbarProgress(this ProgressBar progressBar)
		{
			if (!Windows7Taskbar.Supported) return;

			Form form = (Form)progressBar.TopLevelControl;
			if ((form = FindFormInTaskbar(form)) == null) return;

			Windows7Taskbar.SetProgressState(
					form.Handle, Windows7Taskbar.ThumbnailProgressState.NoProgress);
		}
		/// <summary>
		/// Sets the progress bar in the containing form's taskbar button
		/// to this toolstrip progress bar's progress.
		/// </summary>
		/// <param name="progressBar">The progress bar.</param>
		public static void SetTaskbarProgress( this ToolStripProgressBar progressBar )
		{
			if ( !Windows7Taskbar.Supported ) return;

			Form form = (Form)progressBar.Control.TopLevelControl;
			if ( (form = FindFormInTaskbar( form )) == null ) return;

			//Approximation:
			ulong maximum = (ulong)(progressBar.Maximum - progressBar.Minimum);
			ulong progress = (ulong)(progressBar.Value - progressBar.Minimum);

			Windows7Taskbar.SetProgressState(
					form.Handle, Windows7Taskbar.ThumbnailProgressState.Normal );
			Windows7Taskbar.SetProgressValue(
					form.Handle, progress, maximum );
		}
		/// <summary>
		/// Sets the progress bar of this form's taskbar button to the
		/// specified percentage.
		/// </summary>
		/// <param name="form">The form.</param>
		/// <param name="percent">The progress percentage.</param>
		public static void SetTaskbarProgress( this Form form, float percent )
		{
			if ( !Windows7Taskbar.Supported ) return;

			if ( (form = FindFormInTaskbar( form )) == null ) return;

			Windows7Taskbar.SetProgressState(
					form.Handle, Windows7Taskbar.ThumbnailProgressState.Normal );
			Windows7Taskbar.SetProgressValue(
					form.Handle, (ulong)percent, 100 );
		}
		/// <summary>
		/// Sets the progress bar of this form's taskbar button to the
		/// specified state.
		/// </summary>
		/// <param name="form">The form.</param>
		/// <param name="state">The taskbar progress state.</param>
		public static void SetTaskbarProgressState( this Form form, Windows7Taskbar.ThumbnailProgressState state )
		{
			if ( !Windows7Taskbar.Supported ) return;

			if ( (form = FindFormInTaskbar( form )) == null ) return;

			Windows7Taskbar.SetProgressState( form.Handle, state );
		}
		/// <summary>
		/// Creates a background worker that dispatches progress notifications
		/// to the application's taskbar button.
		/// </summary>
		/// <param name="form">The form.</param>
		/// <returns>An instance of <see cref="BackgroundWorker"/> that reports
		/// progress through the application's taskbar button.</returns>
		public static BackgroundWorker CreateProgressEnabledBackgroundWorker(
				this Form form )
		{
			BackgroundWorker worker = new BackgroundWorker();
			worker.WorkerReportsProgress = true;
			worker.ProgressChanged += ( o, e ) =>
					form.SetTaskbarProgress( e.ProgressPercentage );
			worker.RunWorkerCompleted += ( o, e ) =>
					form.SetTaskbarProgressState( Windows7Taskbar.ThumbnailProgressState.NoProgress );
			return worker;
		}
		/// <summary>
		/// Creates a taskbar thumbnail button manager for this form.
		/// </summary>
		/// <param name="form">The form.</param>
		/// <returns>An object of type <see cref="ThumbButtonManager"/>
		/// that can be used to add and manage thumbnail toolbar buttons.</returns>
		public static ThumbButtonManager CreateThumbButtonManager(
				this Form form )
		{
			if ( !Windows7Taskbar.Supported ) return null;

			return new ThumbButtonManager( form.Handle );
		}
		/// <summary>
		/// Creates a jump list manager for this form.
		/// </summary>
		/// <param name="form">The form.</param>
		/// <returns>An object of type <see cref="JumpListManager"/>
		/// that can be used to manage the application's jump list.</returns>
		public static JumpListManager CreateJumpListManager(
				this Form form )
		{
			if ( !Windows7Taskbar.Supported ) return null;

			return new JumpListManager( form.Handle );
		}

		/// <summary>
		/// Specifies that only a portion of the form's client area
		/// should be used in the form's thumbnail.
		/// </summary>
		/// <param name="form">The form.</param>
		/// <param name="clipRect">The rectangle that specifies the clipped region.</param>
		public static void SetThumbnailClip(
				this Form form, Rectangle clipRect )
		{
			if ( Windows7Taskbar.Supported )
				Windows7Taskbar.SetThumbnailClip( form.Handle, clipRect );
		}

		/// <summary>
		/// Sets the specified form's thumbnail tooltip.
		/// </summary>
		/// <param name="form">The form.</param>
		/// <param name="tooltip">The tooltip text.</param>
		public static void SetThumbnailTooltip( this Form form, string tooltip )
		{
			if ( Windows7Taskbar.Supported )
				Windows7Taskbar.SetThumbnailTooltip( form.Handle, tooltip );
		}
	}
}