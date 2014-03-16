// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Windows7.DesktopIntegration.Interop
{

	internal enum KNOWNDESTCATEGORY
	{
		KDC_FREQUENT = 1,
		KDC_RECENT
	}

	internal enum APPDOCLISTTYPE
	{
		ADLT_RECENT = 0,
		ADLT_FREQUENT
	}

	[StructLayout( LayoutKind.Sequential )]
	internal struct RECT
	{
		public int left;
		public int top;
		public int right;
		public int bottom;

		public RECT( int left, int top, int right, int bottom )
		{
			this.left = left;
			this.top = top;
			this.right = right;
			this.bottom = bottom;
		}
	}

	internal enum TBPFLAG
	{
		TBPF_NOPROGRESS = 0,
		TBPF_INDETERMINATE = 0x1,
		TBPF_NORMAL = 0x2,
		TBPF_ERROR = 0x4,
		TBPF_PAUSED = 0x8
	}

	internal enum TBATFLAG
	{
		TBATF_USEMDITHUMBNAIL = 0x1,
		TBATF_USEMDILIVEPREVIEW = 0x2
	}

	internal enum THBMASK
	{
		THB_BITMAP = 0x1,
		THB_ICON = 0x2,
		THB_TOOLTIP = 0x4,
		THB_FLAGS = 0x8
	}

	internal enum THBFLAGS
	{
		THBF_ENABLED = 0,
		THBF_DISABLED = 0x1,
		THBF_DISMISSONCLICK = 0x2,
		THBF_NOBACKGROUND = 0x4,
		THBF_HIDDEN = 0x8
	}

	[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Auto )]
	internal struct THUMBBUTTON
	{
		[MarshalAs( UnmanagedType.U4 )]
		public THBMASK dwMask;
		public uint iId;
		public uint iBitmap;
		public IntPtr hIcon;
		[MarshalAs( UnmanagedType.ByValTStr, SizeConst = 260 )]
		public string szTip;
		[MarshalAs( UnmanagedType.U4 )]
		public THBFLAGS dwFlags;
	}

	[StructLayout( LayoutKind.Sequential, Pack = 4 )]
	internal struct PropertyKey
	{
		public Guid fmtid;
		public uint pid;

		public PropertyKey( Guid fmtid, uint pid )
		{
			this.fmtid = fmtid;
			this.pid = pid;
		}

		public static PropertyKey PKEY_Title = new PropertyKey( new Guid( "F29F85E0-4FF9-1068-AB91-08002B27B3D9" ), 2 );
		public static PropertyKey PKEY_AppUserModel_ID = new PropertyKey( new Guid( "9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3" ), 5 );
		public static PropertyKey PKEY_AppUserModel_IsDestListSeparator = new PropertyKey( new Guid( "9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3" ), 6 );
		public static PropertyKey PKEY_AppUserModel_RelaunchCommand = new PropertyKey( new Guid( "9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3" ), 2 );
		public static PropertyKey PKEY_AppUserModel_RelaunchDisplayNameResource = new PropertyKey( new Guid( "9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3" ), 4 );
		public static PropertyKey PKEY_AppUserModel_RelaunchIconResource = new PropertyKey( new Guid( "9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3" ), 3 );
	}

	[StructLayout( LayoutKind.Explicit )]
	internal struct CALPWSTR
	{
		[FieldOffset( 0 )]
		internal uint cElems;
		[FieldOffset( 4 )]
		internal IntPtr pElems;
	}

	[StructLayout( LayoutKind.Explicit )]
	internal struct PropVariant
	{
		[FieldOffset( 0 )]
		private ushort vt;
		[FieldOffset( 8 )]
		private IntPtr pointerValue;
		[FieldOffset( 8 )]
		private byte byteValue;
		[FieldOffset( 8 )]
		private long longValue;
		[FieldOffset( 8 )]
		private short boolValue;
		[MarshalAs( UnmanagedType.Struct )]
		[FieldOffset( 8 )]
		private CALPWSTR calpwstr;

		[DllImport( "ole32.dll" )]
		private static extern int PropVariantClear( ref PropVariant pvar );

		public VarEnum VarType
		{
			get { return (VarEnum)vt; }
		}

		public void SetValue( String val )
		{
			this.Clear();
			this.vt = (ushort)VarEnum.VT_LPWSTR;
			this.pointerValue = Marshal.StringToCoTaskMemUni( val );
		}
		public void SetValue( bool val )
		{
			this.Clear();
			this.vt = (ushort)VarEnum.VT_BOOL;
			this.boolValue = val ? (short)-1 : (short)0;
		}

		public string GetValue()
		{
			return Marshal.PtrToStringUni( this.pointerValue );
		}

		public void Clear()
		{
			PropVariantClear( ref this );
		}
	}

	internal enum SHARD
	{
		SHARD_PIDL = 0x1,
		SHARD_PATHA = 0x2,
		SHARD_PATHW = 0x3,
		SHARD_APPIDINFO = 0x4, // indicates the data type is a pointer to a SHARDAPPIDINFO structure
		SHARD_APPIDINFOIDLIST = 0x5, // indicates the data type is a pointer to a SHARDAPPIDINFOIDLIST structure
		SHARD_LINK = 0x6, // indicates the data type is a pointer to an IShellLink instance
		SHARD_APPIDINFOLINK = 0x7, // indicates the data type is a pointer to a SHARDAPPIDINFOLINK structure 
	}

	[StructLayout( LayoutKind.Sequential, Pack = 4 )]
	internal struct SHARDAPPIDINFO
	{
		//[MarshalAs(UnmanagedType.Interface)]
		//public object psi;    // The namespace location of the the item that should be added to the recent docs folder.
		//public IntPtr psi;
		public IShellItem psi;
		[MarshalAs( UnmanagedType.LPWStr )]
		public string pszAppID;  // The id of the application that should be associated with this recent doc.
	}

	//TODO: Test this as well, currently not tested
	[StructLayout( LayoutKind.Sequential, Pack = 4 )]
	internal struct SHARDAPPIDINFOIDLIST
	{
		public IntPtr pidl;      // The idlist for the shell item that should be added to the recent docs folder.
		[MarshalAs( UnmanagedType.LPWStr )]
		public string pszAppID;  // The id of the application that should be associated with this recent doc.
	}

	[StructLayout( LayoutKind.Sequential, Pack = 4 )]
	internal struct SHARDAPPIDINFOLINK
	{
		// An IShellLink instance that when launched opens a recently used item in the specified 
		// application. This link is not added to the recent docs folder, but will be added to the
		// specified application's destination list.
		//public IntPtr psl;
		public IShellLinkW psl;
		[MarshalAs( UnmanagedType.LPWStr )]
		public string pszAppID;  // The id of the application that should be associated with this recent doc.
	}

	[SuppressUnmanagedCodeSecurity]
	internal static class SafeNativeMethods
	{
		//Obviously, these GUIDs shouldn't be modified.  The reason they
		//are not readonly is that they are passed with 'ref' to various
		//native methods.
		public static Guid IID_IObjectArray = new Guid( "92CA9DCD-5622-4BBA-A805-5E9F541BD8C9" );
		public static Guid IID_IObjectCollection = new Guid( "5632B1A4-E38A-400A-928A-D4CD63230295" );
		public static Guid IID_IPropertyStore = new Guid( IIDGuid.IPropertyStore );
		public static Guid IID_IUnknown = new Guid( "00000000-0000-0000-C000-000000000046" );

		public const int DWM_SIT_DISPLAYFRAME = 0x00000001;

		public const int DWMWA_FORCE_ICONIC_REPRESENTATION = 7;
		public const int DWMWA_HAS_ICONIC_BITMAP = 10;
		//TODO: DISALLOW_PEEK and FLIP3D_POLICY etc. sound interesting too

		public const int WM_COMMAND = 0x111;
		public const int WM_SYSCOMMAND = 0x112;
		public const int WM_DWMSENDICONICTHUMBNAIL = 0x0323;
		public const int WM_DWMSENDICONICLIVEPREVIEWBITMAP = 0x0326;
		public const int WM_CLOSE = 0x0010;
		public const int WM_ACTIVATE = 0x0006;

		public const int WA_ACTIVE = 1;
		public const int WA_CLICKACTIVE = 2;

		public const int SC_CLOSE = 0xF060;

		public const int MSGFLT_ADD = 1;
		public const int MSGFLT_REMOVE = 2;

		// Thumbbutton WM_COMMAND notification
		public const uint THBN_CLICKED = 0x1800;


		#region Shell Library

		internal enum LIBRARYFOLDERFILTER
		{
			LFF_FORCEFILESYSTEM = 1,
			LFF_STORAGEITEMS = 2,
			LFF_ALLITEMS = 3
		};

		internal enum LIBRARYOPTIONFLAGS
		{
			LOF_DEFAULT = 0,
			LOF_PINNEDTONAVPANE = 0x1,
			LOF_MASK_ALL = 0x1
		};

		internal enum DEFAULTSAVEFOLDERTYPE
		{
			DSFT_DETECT = 1,
			DSFT_PRIVATE = (DSFT_DETECT + 1),
			DSFT_PUBLIC = (DSFT_PRIVATE + 1)
		};


		internal enum LIBRARYSAVEFLAGS
		{
			LSF_FAILIFTHERE = 0,
			LSF_OVERRIDEEXISTING = 0x1,
			LSF_MAKEUNIQUENAME = 0x2
		};

		internal enum LIBRARYMANAGEDIALOGOPTIONS
		{
			LMD_DEFAULT = 0,
			LMD_NOUNINDEXABLELOCATIONWARNING = 0x1
		};

		internal enum StorageInstantiationModes
		{
			STGM_DIRECT = 0x00000000,
			STGM_TRANSACTED = 0x00010000,
			STGM_SIMPLE = 0x08000000,
			STGM_READ = 0x00000000,
			STGM_WRITE = 0x00000001,
			STGM_READWRITE = 0x00000002,
			STGM_SHARE_DENY_NONE = 0x00000040,
			STGM_SHARE_DENY_READ = 0x00000030,
			STGM_SHARE_DENY_WRITE = 0x00000020,
			STGM_SHARE_EXCLUSIVE = 0x00000010,
			STGM_PRIORITY = 0x00040000,
			STGM_DELETEONRELEASE = 0x04000000,
			STGM_NOSCRATCH = 0x00100000,
			STGM_CREATE = 0x00001000,
			STGM_CONVERT = 0x00020000,
			STGM_FAILIFTHERE = 0x00000000,
			STGM_NOSNAPSHOT = 0x00200000,
			STGM_DIRECT_SWMR = 0x00400000
		};
		#endregion

		#region VistaBridge

		// Various important window messages
		internal const int WM_USER = 0x0400;
		internal const int WM_ENTERIDLE = 0x0121;

		internal enum SIGDN : uint
		{
			SIGDN_NORMALDISPLAY = 0x00000000,           // SHGDN_NORMAL
			SIGDN_PARENTRELATIVEPARSING = 0x80018001,   // SHGDN_INFOLDER | SHGDN_FORPARSING
			SIGDN_DESKTOPABSOLUTEPARSING = 0x80028000,  // SHGDN_FORPARSING
			SIGDN_PARENTRELATIVEEDITING = 0x80031001,   // SHGDN_INFOLDER | SHGDN_FOREDITING
			SIGDN_DESKTOPABSOLUTEEDITING = 0x8004c000,  // SHGDN_FORPARSING | SHGDN_FORADDRESSBAR
			SIGDN_FILESYSPATH = 0x80058000,             // SHGDN_FORPARSING
			SIGDN_URL = 0x80068000,                     // SHGDN_FORPARSING
			SIGDN_PARENTRELATIVEFORADDRESSBAR = 0x8007c001,     // SHGDN_INFOLDER | SHGDN_FORPARSING | SHGDN_FORADDRESSBAR
			SIGDN_PARENTRELATIVE = 0x80080001           // SHGDN_INFOLDER
		}

		[StructLayout( LayoutKind.Sequential )]
		internal struct POINT
		{
			internal int X;
			internal int Y;

			internal POINT( int x, int y )
			{
				this.X = x;
				this.Y = y;
			}
		}

		// Property System structs and consts.
		[StructLayout( LayoutKind.Sequential, Pack = 4 )]
		internal struct PROPERTYKEY
		{
			internal Guid fmtid;
			internal uint pid;
		}

		internal enum SIATTRIBFLAGS
		{
			// if multiple items and the attirbutes together.
			SIATTRIBFLAGS_AND = 0x00000001,
			// if multiple items or the attributes together.
			SIATTRIBFLAGS_OR = 0x00000002,
			// Call GetAttributes directly on the 
			// ShellFolder for multiple attributes.
			SIATTRIBFLAGS_APPCOMPAT = 0x00000003,
		}

		#region Task Dialog Declarations

		// Main task dialog configuration struct.
		// NOTE: Packing must be set to 4 to make this work on 64-bit platforms.
		[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4 )]
		internal class TASKDIALOGCONFIG
		{
			internal uint cbSize;
			internal IntPtr hwndParent;
			internal IntPtr hInstance;
			internal TASKDIALOG_FLAGS dwFlags;
			internal TASKDIALOG_COMMON_BUTTON_FLAGS dwCommonButtons;
			[MarshalAs( UnmanagedType.LPWStr )]
			internal string pszWindowTitle;
			internal TASKDIALOGCONFIG_ICON_UNION MainIcon; // NOTE: 32-bit union field, holds pszMainIcon as well
			[MarshalAs( UnmanagedType.LPWStr )]
			internal string pszMainInstruction;
			[MarshalAs( UnmanagedType.LPWStr )]
			internal string pszContent;
			internal uint cButtons;
			internal IntPtr pButtons;           // Ptr to TASKDIALOG_BUTTON structs
			internal int nDefaultButton;
			internal uint cRadioButtons;
			internal IntPtr pRadioButtons;      // Ptr to TASKDIALOG_BUTTON structs
			internal int nDefaultRadioButton;
			[MarshalAs( UnmanagedType.LPWStr )]
			internal string pszVerificationText;
			[MarshalAs( UnmanagedType.LPWStr )]
			internal string pszExpandedInformation;
			[MarshalAs( UnmanagedType.LPWStr )]
			internal string pszExpandedControlText;
			[MarshalAs( UnmanagedType.LPWStr )]
			internal string pszCollapsedControlText;
			internal TASKDIALOGCONFIG_ICON_UNION FooterIcon;  // NOTE: 32-bit union field, holds pszFooterIcon as well
			[MarshalAs( UnmanagedType.LPWStr )]
			internal string pszFooter;
			internal PFTASKDIALOGCALLBACK pfCallback;
			internal IntPtr lpCallbackData;
			internal uint cxWidth;
		}

		internal const int TASKDIALOG_IDEALWIDTH = 0;  // Value for TASKDIALOGCONFIG.cxWidth
		internal const int TASKDIALOG_BUTTON_SHIELD_ICON = 1;

		// NOTE: We include a "spacer" so that the struct size varies on 
		// 64-bit architectures.
		[StructLayout( LayoutKind.Explicit, CharSet = CharSet.Auto )]
		internal struct TASKDIALOGCONFIG_ICON_UNION
		{
			internal TASKDIALOGCONFIG_ICON_UNION( int i )
			{
				spacer = IntPtr.Zero;
				pszIcon = 0;
				hMainIcon = i;
			}

			[FieldOffset( 0 )]
			internal int hMainIcon;
			[FieldOffset( 0 )]
			internal int pszIcon;
			[FieldOffset( 0 )]
			internal IntPtr spacer;
		}

		// NOTE: Packing must be set to 4 to make this work on 64-bit platforms.
		[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4 )]
		internal struct TASKDIALOG_BUTTON
		{
			public TASKDIALOG_BUTTON( int n, string txt )
			{
				nButtonID = n;
				pszButtonText = txt;
			}

			internal int nButtonID;
			[MarshalAs( UnmanagedType.LPWStr )]
			internal string pszButtonText;
		}

		// Task Dialog - identifies common buttons.
		[Flags]
		internal enum TASKDIALOG_COMMON_BUTTON_FLAGS
		{
			TDCBF_OK_BUTTON = 0x0001, // selected control return value IDOK
			TDCBF_YES_BUTTON = 0x0002, // selected control return value IDYES
			TDCBF_NO_BUTTON = 0x0004, // selected control return value IDNO
			TDCBF_CANCEL_BUTTON = 0x0008, // selected control return value IDCANCEL
			TDCBF_RETRY_BUTTON = 0x0010, // selected control return value IDRETRY
			TDCBF_CLOSE_BUTTON = 0x0020  // selected control return value IDCLOSE
		}

		// Identify button *return values* - note that, unfortunately, these are different
		// from the inbound button values.
		internal enum TASKDIALOG_COMMON_BUTTON_RETURN_ID
		{
			IDOK = 1,
			IDCANCEL = 2,
			IDABORT = 3,
			IDRETRY = 4,
			IDIGNORE = 5,
			IDYES = 6,
			IDNO = 7,
			IDCLOSE = 8
		}

		internal enum TASKDIALOG_ELEMENTS
		{
			TDE_CONTENT,
			TDE_EXPANDED_INFORMATION,
			TDE_FOOTER,
			TDE_MAIN_INSTRUCTION
		}

		internal enum TASKDIALOG_ICON_ELEMENT
		{
			TDIE_ICON_MAIN,
			TDIE_ICON_FOOTER
		}

		// Task Dialog - flags
		[Flags]
		internal enum TASKDIALOG_FLAGS
		{
			NONE = 0,
			TDF_ENABLE_HYPERLINKS = 0x0001,
			TDF_USE_HICON_MAIN = 0x0002,
			TDF_USE_HICON_FOOTER = 0x0004,
			TDF_ALLOW_DIALOG_CANCELLATION = 0x0008,
			TDF_USE_COMMAND_LINKS = 0x0010,
			TDF_USE_COMMAND_LINKS_NO_ICON = 0x0020,
			TDF_EXPAND_FOOTER_AREA = 0x0040,
			TDF_EXPANDED_BY_DEFAULT = 0x0080,
			TDF_VERIFICATION_FLAG_CHECKED = 0x0100,
			TDF_SHOW_PROGRESS_BAR = 0x0200,
			TDF_SHOW_MARQUEE_PROGRESS_BAR = 0x0400,
			TDF_CALLBACK_TIMER = 0x0800,
			TDF_POSITION_RELATIVE_TO_WINDOW = 0x1000,
			TDF_RTL_LAYOUT = 0x2000,
			TDF_NO_DEFAULT_RADIO_BUTTON = 0x4000
		}

		internal enum TASKDIALOG_MESSAGES
		{
			TDM_NAVIGATE_PAGE = WM_USER + 101,
			TDM_CLICK_BUTTON = WM_USER + 102, // wParam = Button ID
			TDM_SET_MARQUEE_PROGRESS_BAR = WM_USER + 103, // wParam = 0 (nonMarque) wParam != 0 (Marquee)
			TDM_SET_PROGRESS_BAR_STATE = WM_USER + 104, // wParam = new progress state
			TDM_SET_PROGRESS_BAR_RANGE = WM_USER + 105, // lParam = MAKELPARAM(nMinRange, nMaxRange)
			TDM_SET_PROGRESS_BAR_POS = WM_USER + 106, // wParam = new position
			TDM_SET_PROGRESS_BAR_MARQUEE = WM_USER + 107, // wParam = 0 (stop marquee), wParam != 0 (start marquee), lparam = speed (milliseconds between repaints)
			TDM_SET_ELEMENT_TEXT = WM_USER + 108, // wParam = element (TASKDIALOG_ELEMENTS), lParam = new element text (LPCWSTR)
			TDM_CLICK_RADIO_BUTTON = WM_USER + 110, // wParam = Radio Button ID
			TDM_ENABLE_BUTTON = WM_USER + 111, // lParam = 0 (disable), lParam != 0 (enable), wParam = Button ID
			TDM_ENABLE_RADIO_BUTTON = WM_USER + 112, // lParam = 0 (disable), lParam != 0 (enable), wParam = Radio Button ID
			TDM_CLICK_VERIFICATION = WM_USER + 113, // wParam = 0 (unchecked), 1 (checked), lParam = 1 (set key focus)
			TDM_UPDATE_ELEMENT_TEXT = WM_USER + 114, // wParam = element (TASKDIALOG_ELEMENTS), lParam = new element text (LPCWSTR)
			TDM_SET_BUTTON_ELEVATION_REQUIRED_STATE = WM_USER + 115, // wParam = Button ID, lParam = 0 (elevation not required), lParam != 0 (elevation required)
			TDM_UPDATE_ICON = WM_USER + 116  // wParam = icon element (TASKDIALOG_ICON_ELEMENTS), lParam = new icon (hIcon if TDF_USE_HICON_* was set, PCWSTR otherwise)
		}

		internal enum TASKDIALOG_NOTIFICATIONS
		{
			TDN_CREATED = 0,
			TDN_NAVIGATED = 1,
			TDN_BUTTON_CLICKED = 2,            // wParam = Button ID
			TDN_HYPERLINK_CLICKED = 3,         // lParam = (LPCWSTR)pszHREF
			TDN_TIMER = 4,                     // wParam = Milliseconds since dialog created or timer reset
			TDN_DESTROYED = 5,
			TDN_RADIO_BUTTON_CLICKED = 6,      // wParam = Radio Button ID
			TDN_DIALOG_CONSTRUCTED = 7,
			TDN_VERIFICATION_CLICKED = 8,      // wParam = 1 if checkbox checked, 0 if not, lParam is unused and always 0
			TDN_HELP = 9,
			TDN_EXPANDO_BUTTON_CLICKED = 10    // wParam = 0 (dialog is now collapsed), wParam != 0 (dialog is now expanded)
		}

		// Used in the various SET_DEFAULT* TaskDialog messages
		internal const int NO_DEFAULT_BUTTON_SPECIFIED = 0;

		// Task Dialog config and related structs (for TaskDialogIndirect())
		internal delegate int PFTASKDIALOGCALLBACK(
				IntPtr hwnd,
				uint msg,
				IntPtr wParam,
				IntPtr lParam,
				IntPtr lpRefData );

		internal enum PBST
		{
			PBST_NORMAL = 0x0001,
			PBST_ERROR = 0x0002,
			PBST_PAUSED = 0x0003
		}

		internal enum TD_ICON
		{
			TD_WARNING_ICON = 65535,
			TD_ERROR_ICON = 65534,
			TD_INFORMATION_ICON = 65533,
			TD_SHIELD_ICON = 65532
		}

		#endregion

		#endregion
	}

	[SuppressUnmanagedCodeSecurity]
	internal static class UnsafeNativeMethods
	{
		[DllImport( "shell32.dll" )]
		public static extern int SHGetPropertyStoreForWindow(
				IntPtr hwnd,
				ref Guid iid /*IID_IPropertyStore*/,
				[Out(), MarshalAs( UnmanagedType.Interface )]
				out IPropertyStore propertyStore );

		[DllImport( "dwmapi.dll" )]
		public static extern int DwmSetIconicThumbnail(
				IntPtr hwnd, IntPtr hbitmap, uint flags );

		[DllImport( "dwmapi.dll" )]
		public static extern int DwmSetIconicLivePreviewBitmap(
				IntPtr hwnd,
				IntPtr hbitmap,
				ref /*VistaBridgeInterop.*/SafeNativeMethods.POINT ptClient,
				uint flags );
		[DllImport( "dwmapi.dll" )]
		public static extern int DwmSetIconicLivePreviewBitmap(
				IntPtr hwnd, IntPtr hbitmap, IntPtr ptClient, uint flags );

		[DllImport( "dwmapi.dll" )]
		public static extern int DwmInvalidateIconicBitmaps( IntPtr hwnd );

		[DllImport( "user32.dll" )]
		[return: MarshalAs( UnmanagedType.Bool )]
		public static extern bool ChangeWindowMessageFilter( uint message, uint flags );

		[DllImport( "user32.dll", SetLastError = true, CharSet = CharSet.Auto )]
		internal static extern uint RegisterWindowMessage( string lpString );
		public static uint WM_TaskbarButtonCreated
		{
			get
			{
				if ( _uTBBCMsg == 0 )
				{
					_uTBBCMsg = RegisterWindowMessage( "TaskbarButtonCreated" );
				}
				return _uTBBCMsg;
			}
		}

		private static uint _uTBBCMsg;

		[DllImport( "shell32.dll" )]
		public static extern void SetCurrentProcessExplicitAppUserModelID(
				[MarshalAs( UnmanagedType.LPWStr )] string AppID );
		[DllImport( "shell32.dll" )]
		public static extern void GetCurrentProcessExplicitAppUserModelID(
				[Out(), MarshalAs( UnmanagedType.LPWStr )] out string AppID );

		[DllImport( "shell32.dll" )]
		public static extern void SHAddToRecentDocs( SHARD flags, IntPtr pv );

		[DllImport( "shell32.dll" )]
		public static extern void SHAddToRecentDocs(
				SHARD flags,
				[MarshalAs( UnmanagedType.LPWStr )] string path );
		public static void SHAddToRecentDocs( string path )
		{
			UnsafeNativeMethods.SHAddToRecentDocs( SHARD.SHARD_PATHW, path );
		}

		[DllImport( "shell32.dll" )]
		public static extern void SHAddToRecentDocs(
				SHARD flags,
				ref SHARDAPPIDINFO appIDInfo );
		public static void SHAddToRecentDocs( ref SHARDAPPIDINFO appIDInfo )
		{
			UnsafeNativeMethods.SHAddToRecentDocs( SHARD.SHARD_APPIDINFO, ref appIDInfo );
		}

		[DllImport( "shell32.dll" )]
		public static extern void SHAddToRecentDocs(
				SHARD flags,
				[MarshalAs( UnmanagedType.LPStruct )] ref SHARDAPPIDINFOIDLIST appIDInfoIDList );
		public static void SHAddToRecentDocs( ref SHARDAPPIDINFOIDLIST appIDInfoIDList )
		{
			UnsafeNativeMethods.SHAddToRecentDocs( SHARD.SHARD_APPIDINFOIDLIST, ref appIDInfoIDList );
		}

		[DllImport( "shell32.dll" )]
		public static extern void SHAddToRecentDocs(
				SHARD flags,
				ref SHARDAPPIDINFOLINK appIDInfoLink );
		public static void SHAddToRecentDocs( ref SHARDAPPIDINFOLINK appIDInfoLink )
		{
			UnsafeNativeMethods.SHAddToRecentDocs( SHARD.SHARD_APPIDINFOLINK, ref appIDInfoLink );
		}

		[DllImport( "user32.dll" )]
		[return: MarshalAs( UnmanagedType.Bool )]
		public static extern bool GetWindowRect( IntPtr hwnd, ref RECT rect );

		[DllImport( "user32.dll" )]
		[return: MarshalAs( UnmanagedType.Bool )]
		public static extern bool GetClientRect( IntPtr hwnd, ref RECT rect );

		public static bool GetClientSize( IntPtr hwnd, out System.Drawing.Size size )
		{
			RECT rect = new RECT();
			if ( !GetClientRect( hwnd, ref rect ) )
			{
				size = new System.Drawing.Size( -1, -1 );
				return false;
			}
			size = new System.Drawing.Size( rect.right, rect.bottom );
			return true;
		}

		[DllImport( "user32.dll" )]
		[return: MarshalAs( UnmanagedType.Bool )]
		public static extern bool ShowWindow( IntPtr hwnd, int cmd );

		[DllImport( "user32.dll" )]
		[return: MarshalAs( UnmanagedType.Bool )]
		public static extern bool SetWindowPos( IntPtr hwnd, IntPtr hwndInsertAfter, int X, int Y, int cx, int cy, uint flags );

		[DllImport( "user32.dll" )]
		[return: MarshalAs( UnmanagedType.Bool )]
		public static extern bool ClientToScreen(
				IntPtr hwnd,
				ref SafeNativeMethods.POINT point );

		[DllImport( "user32.dll" )]
		public static extern int GetWindowText(
				IntPtr hwnd, StringBuilder str, int maxCount );

		[DllImport( "gdi32.dll" )]
		[return: MarshalAs( UnmanagedType.Bool )]
		public static extern bool BitBlt(
				IntPtr hDestDC, int destX, int destY, int width, int height,
				IntPtr hSrcDC, int srcX, int srcY,
				uint operation );

		[DllImport( "gdi32.dll" )]
		[return: MarshalAs( UnmanagedType.Bool )]
		public static extern bool StretchBlt(
				IntPtr hDestDC, int destX, int destY, int destWidth, int destHeight,
				IntPtr hSrcDC, int srcX, int srcY, int srcWidth, int srcHeight,
				uint operation );

		[DllImport( "user32.dll" )]
		public static extern IntPtr GetWindowDC( IntPtr hwnd );

		[DllImport( "user32.dll" )]
		public static extern int ReleaseDC( IntPtr hwnd, IntPtr hdc );

		#region VistaBridge
		[DllImport( "DwmApi.dll" )]
		internal static extern int DwmSetWindowAttribute(
				IntPtr hwnd,
			//DWMWA_* values.
				uint dwAttributeToSet,
				IntPtr pvAttributeValue,
				uint cbAttribute );
		#endregion

	}

	internal static class NativeLibraryMethods
	{
		[DllImport( "Shell32", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi, SetLastError = true )]
		internal static extern int SHShowManageLibraryUI(
																		[In, MarshalAs( UnmanagedType.Interface )] /*Microsoft.SDK.Samples.VistaBridge.Interop.*/IShellItem library,
																		[In] IntPtr hwndOwner,
																		[In] string title,
																		[In] string instruction,
																		[In] SafeNativeMethods.LIBRARYMANAGEDIALOGOPTIONS lmdOptions );

	}

	internal static class Helpers
	{

		internal static IShellItem GetShellItemFromPath( string path )
		{
			if ( String.IsNullOrEmpty( path ) )
				throw new ArgumentNullException(
				"path", "Shell item cannot be generated from null or empty path." );

			IShellItem resultItem;
			Guid shellItemGuid = new Guid( IIDGuid.IShellItem );
			uint result = NativeMethods.SHCreateItemFromParsingName(
					path,
					IntPtr.Zero,
					ref shellItemGuid,
					out resultItem );
			// Throw if an error occurred.
			System.Runtime.InteropServices.Marshal.ThrowExceptionForHR( (int)result );
			return resultItem;
		}
	}

	/// <summary>
	/// Internal class containing most native interop declarations used
	/// throughout the library.
	/// Functions that are not performance intensive belong in this class.
	/// </summary>

	internal static class NativeMethods
	{
		#region General Definitions

		// Various helpers for forcing binding to proper 
		// version of Comctl32 (v6).
		[DllImport( ExternDll.Kernel32, SetLastError = true,
		ThrowOnUnmappableChar = true, BestFitMapping = false )]
		internal static extern IntPtr LoadLibrary(
				 [MarshalAs( UnmanagedType.LPStr )] string lpFileName );

		[DllImport( ExternDll.Kernel32, SetLastError = true,
				ThrowOnUnmappableChar = true, BestFitMapping = false )]
		internal static extern IntPtr GetProcAddress(
				IntPtr hModule,
				[MarshalAs( UnmanagedType.LPStr )] string lpProcName );

		[DllImport( "user32.dll" )]
		[return: MarshalAs( UnmanagedType.Bool )]
		internal static extern bool DeleteObject( IntPtr graphicsObjectHandle );

		#endregion

		#region TaskDialog Definitions

		[DllImport( ExternDll.ComCtl32, CharSet = CharSet.Auto,
				SetLastError = true )]
		internal static extern HRESULT TaskDialog(
				IntPtr hwndParent,
				IntPtr hInstance,
				[MarshalAs( UnmanagedType.LPWStr )] string pszWindowtitle,
				[MarshalAs( UnmanagedType.LPWStr )] string pszMainInstruction,
				[MarshalAs( UnmanagedType.LPWStr )] string pszContent,
				SafeNativeMethods.TASKDIALOG_COMMON_BUTTON_FLAGS dwCommonButtons,
				[MarshalAs( UnmanagedType.LPWStr )]string pszIcon,
				[In, Out] ref int pnButton );

		[DllImport( ExternDll.ComCtl32, CharSet = CharSet.Auto,
				SetLastError = true )]
		internal static extern HRESULT TaskDialogIndirect(
				[In] SafeNativeMethods.TASKDIALOGCONFIG pTaskConfig,
				[Out] out int pnButton,
				[Out] out int pnRadioButton,
				[MarshalAs( UnmanagedType.Bool )][Out] out bool pVerificationFlagChecked );

		internal delegate HRESULT TDIDelegate(
				[In] SafeNativeMethods.TASKDIALOGCONFIG pTaskConfig,
				[Out] out int pnButton,
				[Out] out int pnRadioButton,
				[Out] out bool pVerificationFlagChecked );


		#endregion

		#region Shell Definitions

		[DllImport( ExternDll.Shell32, CharSet = CharSet.Auto,
				SetLastError = true )]
		internal static extern uint SHCreateItemFromParsingName(
				[MarshalAs( UnmanagedType.LPWStr )] string path,
			// The following parameter is not used - binding context.
				IntPtr pbc,
				ref Guid riid,
				[MarshalAs( UnmanagedType.Interface )] out IShellItem shellItem );
		#endregion
	}

	internal enum HRESULT : long
	{
		S_FALSE = 0x0001,
		S_OK = 0x0000,
		E_INVALIDARG = 0x80070057,
		E_OUTOFMEMORY = 0x8007000E
	}

	internal class ExternDll
	{
		internal const string ComCtl32 = "comctl32.dll";
		internal const string Kernel32 = "kernel32.dll";
		internal const string ComDlg32 = "comdlg32.dll";
		internal const string User32 = "user32.dll";
		internal const string Shell32 = "shell32.dll";
	}

}