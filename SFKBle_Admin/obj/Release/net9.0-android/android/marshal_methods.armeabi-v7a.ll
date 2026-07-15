; ModuleID = 'marshal_methods.armeabi-v7a.ll'
source_filename = "marshal_methods.armeabi-v7a.ll"
target datalayout = "e-m:e-p:32:32-Fi8-i64:64-v128:64:128-a:0:32-n32-S64"
target triple = "armv7-unknown-linux-android21"

%struct.MarshalMethodName = type {
	i64, ; uint64_t id
	ptr ; char* name
}

%struct.MarshalMethodsManagedClass = type {
	i32, ; uint32_t token
	ptr ; MonoClass klass
}

@assembly_image_cache = dso_local local_unnamed_addr global [173 x ptr] zeroinitializer, align 4

; Each entry maps hash of an assembly name to an index into the `assembly_image_cache` array
@assembly_image_cache_hashes = dso_local local_unnamed_addr constant [519 x i32] [
	i32 u0x0027eb9e, ; 0: System.Net.NetworkInformation.dll => 137
	i32 u0x00345a11, ; 1: lib_System.Net.Requests.dll.so => 139
	i32 u0x009b21bb, ; 2: System.Net.NameResolution.dll => 136
	i32 u0x00c8cc5d, ; 3: lib_Xamarin.AndroidX.Loader.dll.so => 90
	i32 u0x0119bc86, ; 4: lib_Microsoft.Extensions.DependencyInjection.Abstractions.dll.so => 44
	i32 u0x021422de, ; 5: lib_Syncfusion.Maui.Popup.dll.so => 70
	i32 u0x0254c520, ; 6: Newtonsoft.Json.dll => 56
	i32 u0x025a8054, ; 7: System.Net.WebSockets.dll => 144
	i32 u0x02664405, ; 8: lib-uk-Microsoft.Maui.Controls.resources.dll.so => 29
	i32 u0x028aa24d, ; 9: System.Threading.Thread => 161
	i32 u0x03358480, ; 10: lib_Microsoft.Maui.dll.so => 53
	i32 u0x0335cdbc, ; 11: ca/Microsoft.Maui.Controls.resources => 1
	i32 u0x041e6096, ; 12: en-US/Syncfusion.Maui.Buttons.resources => 34
	i32 u0x044bb714, ; 13: Microsoft.Maui.Graphics.dll => 55
	i32 u0x056606a6, ; 14: lib_System.Collections.NonGeneric.dll.so => 113
	i32 u0x065dd880, ; 15: lib_System.Linq.Queryable.dll.so => 131
	i32 u0x06c2cd46, ; 16: zh-HK/Microsoft.Maui.Controls.resources => 31
	i32 u0x06ffddbc, ; 17: System.Runtime.InteropServices => 150
	i32 u0x074aea82, ; 18: System.Threading.Channels.dll => 160
	i32 u0x0881c32f, ; 19: System.Net.WebHeaderCollection => 142
	i32 u0x0a0c2bd0, ; 20: lib_Xamarin.AndroidX.Activity.dll.so => 74
	i32 u0x0a4f2d15, ; 21: Syncfusion.Maui.Core.dll => 64
	i32 u0x0ade3a75, ; 22: Xamarin.AndroidX.SwipeRefreshLayout.dll => 97
	i32 u0x0aee6a3d, ; 23: lib-vi-Microsoft.Maui.Controls.resources.dll.so => 30
	i32 u0x0aeedc53, ; 24: lib_Xamarin.Google.Android.Material.dll.so => 101
	i32 u0x0b63b1e1, ; 25: lib_System.Net.Http.Json.dll.so => 134
	i32 u0x0b721a36, ; 26: lib-pl-Microsoft.Maui.Controls.resources.dll.so => 20
	i32 u0x0ba65f85, ; 27: vi/Microsoft.Maui.Controls.resources.dll => 30
	i32 u0x0be195c3, ; 28: zh-HK/Microsoft.Maui.Controls.resources.dll => 31
	i32 u0x0c38ff48, ; 29: System.ComponentModel => 118
	i32 u0x0c7b2e71, ; 30: Xamarin.AndroidX.Browser.dll => 77
	i32 u0x0d73bff4, ; 31: lib_Microsoft.Extensions.Logging.Debug.dll.so => 47
	i32 u0x0dc10265, ; 32: Microsoft.CSharp.dll => 110
	i32 u0x0dc2f416, ; 33: lib_Xamarin.AndroidX.CustomView.dll.so => 83
	i32 u0x0e762ada, ; 34: lib-nb-Microsoft.Maui.Controls.resources.dll.so => 18
	i32 u0x0edddcf6, ; 35: lib_Plugin.LocalNotification.dll.so => 58
	i32 u0x0f77782a, ; 36: Syncfusion.Maui.Charts => 63
	i32 u0x10bf9929, ; 37: cs/Microsoft.Maui.Controls.resources.dll => 2
	i32 u0x113d3381, ; 38: lib-sk-Microsoft.Maui.Controls.resources.dll.so => 25
	i32 u0x117ea27e, ; 39: lib_PINView.Maui.dll.so => 57
	i32 u0x13031348, ; 40: Xamarin.AndroidX.Activity.dll => 74
	i32 u0x136bf828, ; 41: lib_System.Runtime.dll.so => 155
	i32 u0x14095832, ; 42: ja/Microsoft.Maui.Controls.resources.dll => 15
	i32 u0x153e1455, ; 43: it/Microsoft.Maui.Controls.resources.dll => 14
	i32 u0x15502fa0, ; 44: cs/Microsoft.Maui.Controls.resources => 2
	i32 u0x15c177ae, ; 45: lib_Microsoft.Extensions.Configuration.dll.so => 41
	i32 u0x15e184df, ; 46: lib_System.Runtime.Loader.dll.so => 151
	i32 u0x16508992, ; 47: Syncfusion.Maui.Popup.dll => 70
	i32 u0x16786ef8, ; 48: en-US/Syncfusion.Maui.Buttons.resources.dll => 34
	i32 u0x16a510e1, ; 49: System.Threading.Thread.dll => 161
	i32 u0x16fe439a, ; 50: System.Memory.dll => 133
	i32 u0x17969339, ; 51: _Microsoft.Android.Resource.Designer => 36
	i32 u0x19f6996b, ; 52: sv/Microsoft.Maui.Controls.resources.dll => 26
	i32 u0x1a61054f, ; 53: System.Collections => 115
	i32 u0x1ae0ec2c, ; 54: Xamarin.AndroidX.Fragment.dll => 85
	i32 u0x1b317bfd, ; 55: System.Web.HttpUtility.dll => 163
	i32 u0x1b3868b3, ; 56: Syncfusion.Maui.GridCommon.dll => 67
	i32 u0x1b5932ea, ; 57: lib_Mono.Android.Runtime.dll.so => 171
	i32 u0x1b611806, ; 58: System.Runtime.Serialization.Primitives.dll => 154
	i32 u0x1bc6ffe7, ; 59: lib_Java.Interop.dll.so => 169
	i32 u0x1bff388e, ; 60: System.dll => 166
	i32 u0x1c78d08a, ; 61: lib_System.Private.Uri.dll.so => 147
	i32 u0x1dbae811, ; 62: System.ObjectModel => 146
	i32 u0x1dd2dc50, ; 63: id/Microsoft.Maui.Controls.resources.dll => 13
	i32 u0x1e092f31, ; 64: fi/Microsoft.Maui.Controls.resources.dll => 7
	i32 u0x1e0ca050, ; 65: Plugin.LocalNotification.dll => 58
	i32 u0x1e9789de, ; 66: Microsoft.Extensions.Primitives.dll => 49
	i32 u0x1f6bf43d, ; 67: hi/Microsoft.Maui.Controls.resources => 10
	i32 u0x1f9b4faa, ; 68: System.Linq.Queryable => 131
	i32 u0x20216150, ; 69: Microsoft.Extensions.Logging => 45
	i32 u0x21e38d8e, ; 70: Syncfusion.Maui.ProgressBar.dll => 71
	i32 u0x234b6fb2, ; 71: pt-BR/Microsoft.Maui.Controls.resources.dll => 21
	i32 u0x2397454a, ; 72: lib_System.Collections.Specialized.dll.so => 114
	i32 u0x239cf51b, ; 73: CommunityToolkit.Maui => 37
	i32 u0x2459aaf0, ; 74: lib_System.Net.Sockets.dll.so => 141
	i32 u0x252c9491, ; 75: Syncfusion.Maui.ProgressBar => 71
	i32 u0x2568904f, ; 76: Xamarin.AndroidX.CustomView => 83
	i32 u0x262d781c, ; 77: lib-de-Microsoft.Maui.Controls.resources.dll.so => 4
	i32 u0x2645b6c3, ; 78: lib_CommunityToolkit.Maui.Core.dll.so => 38
	i32 u0x27787397, ; 79: System.Text.Encodings.Web.dll => 157
	i32 u0x278c7790, ; 80: Xamarin.AndroidX.VersionedParcelable => 98
	i32 u0x27b53050, ; 81: lib_System.Data.Common.dll.so => 120
	i32 u0x2814a96c, ; 82: System.Collections.Concurrent => 111
	i32 u0x28607aa1, ; 83: lib-pt-BR-Microsoft.Maui.Controls.resources.dll.so => 21
	i32 u0x28bdabca, ; 84: System.Net.Security => 140
	i32 u0x2904cf94, ; 85: ca/Microsoft.Maui.Controls.resources.dll => 1
	i32 u0x29293ff5, ; 86: System.Xml.Linq.dll => 164
	i32 u0x29423679, ; 87: lib_Xamarin.AndroidX.CursorAdapter.dll.so => 82
	i32 u0x2973baeb, ; 88: Syncfusion.Maui.Popup => 70
	i32 u0x2a1e8ecb, ; 89: ko/Microsoft.Maui.Controls.resources.dll => 16
	i32 u0x2a4afd4a, ; 90: de/Microsoft.Maui.Controls.resources.dll => 4
	i32 u0x2b15ed29, ; 91: System.Runtime.Loader.dll => 151
	i32 u0x2bcfb3d9, ; 92: lib_Plugin.Maui.Audio.dll.so => 59
	i32 u0x2d445acd, ; 93: System.Net.Requests => 139
	i32 u0x2e394f87, ; 94: System.IO.Compression => 128
	i32 u0x2f0980eb, ; 95: Microsoft.Extensions.Options => 48
	i32 u0x2ff6fb9f, ; 96: System.Data.Common => 120
	i32 u0x30a0e95c, ; 97: lib_System.Threading.Thread.dll.so => 161
	i32 u0x311247b5, ; 98: System.Private.Uri.dll => 147
	i32 u0x317d5b75, ; 99: System.IO.Compression.Brotli => 127
	i32 u0x3312831d, ; 100: lib_Xamarin.AndroidX.DrawerLayout.dll.so => 84
	i32 u0x33e88be1, ; 101: ar/Microsoft.Maui.Controls.resources => 0
	i32 u0x3463c971, ; 102: System.Net.Http.Json => 134
	i32 u0x35e25008, ; 103: System.ComponentModel.Primitives.dll => 116
	i32 u0x3635f196, ; 104: lib_Xamarin.GooglePlayServices.Basement.dll.so => 103
	i32 u0x373f6a31, ; 105: tr/Microsoft.Maui.Controls.resources.dll => 28
	i32 u0x37ea9cd7, ; 106: lib_Xamarin.AndroidX.Lifecycle.ViewModel.Android.dll.so => 88
	i32 u0x38d89c1d, ; 107: lib_Xamarin.AndroidX.Lifecycle.Common.Jvm.dll.so => 86
	i32 u0x38f24a24, ; 108: Newtonsoft.Json => 56
	i32 u0x3b2c715c, ; 109: System.Collections.dll => 115
	i32 u0x3b3271e4, ; 110: zh-Hans/Microsoft.Maui.Controls.resources => 32
	i32 u0x3b4797e5, ; 111: es/Microsoft.Maui.Controls.resources => 6
	i32 u0x3c5e5b62, ; 112: Xamarin.AndroidX.SavedState.dll => 96
	i32 u0x3cbffa41, ; 113: System.Drawing => 125
	i32 u0x3d548d92, ; 114: Microsoft.Extensions.DependencyInjection.Abstractions => 44
	i32 u0x3d5a6611, ; 115: da/Microsoft.Maui.Controls.resources.dll => 3
	i32 u0x3dbaaf8f, ; 116: Xamarin.AndroidX.AppCompat => 75
	i32 u0x3dc84a49, ; 117: System.Drawing.Primitives.dll => 124
	i32 u0x3e444eb4, ; 118: System.Linq.Expressions.dll => 130
	i32 u0x3ebd41f6, ; 119: lib_System.Collections.dll.so => 115
	i32 u0x3eea4db8, ; 120: lib_Microsoft.Extensions.Primitives.dll.so => 49
	i32 u0x408b17f4, ; 121: System.ComponentModel.TypeConverter => 117
	i32 u0x409e66d8, ; 122: Xamarin.Kotlin.StdLib => 106
	i32 u0x41761b2c, ; 123: System => 166
	i32 u0x42be2972, ; 124: lib_System.Text.Encodings.Web.dll.so => 157
	i32 u0x43362f15, ; 125: Microsoft.Extensions.Logging.Debug => 47
	i32 u0x4393e151, ; 126: lib-th-Microsoft.Maui.Controls.resources.dll.so => 27
	i32 u0x444e5c8e, ; 127: lib_System.ComponentModel.TypeConverter.dll.so => 117
	i32 u0x4474042c, ; 128: lib_System.Numerics.Vectors.dll.so => 145
	i32 u0x44845810, ; 129: lib_System.Net.Http.dll.so => 135
	i32 u0x4626bce1, ; 130: Syncfusion.Maui.TabView => 73
	i32 u0x463a8801, ; 131: Xamarin.AndroidX.Navigation.Runtime.dll => 93
	i32 u0x464305ed, ; 132: fi/Microsoft.Maui.Controls.resources => 7
	i32 u0x47b79c15, ; 133: pl/Microsoft.Maui.Controls.resources.dll => 20
	i32 u0x480a69ad, ; 134: System.Diagnostics.Process => 122
	i32 u0x499b8219, ; 135: nb/Microsoft.Maui.Controls.resources.dll => 18
	i32 u0x4a0189ae, ; 136: lib-hi-Microsoft.Maui.Controls.resources.dll.so => 10
	i32 u0x4a4cd262, ; 137: Xamarin.AndroidX.Collection.Jvm.dll => 79
	i32 u0x4a59a8db, ; 138: PINView.Maui => 57
	i32 u0x4ae97402, ; 139: lib_Microsoft.Maui.Graphics.dll.so => 55
	i32 u0x4b275854, ; 140: Xamarin.KotlinX.Serialization.Core.Jvm => 108
	i32 u0x4b863c7a, ; 141: lib_System.Private.Xml.Linq.dll.so => 148
	i32 u0x4d14ee2b, ; 142: Xamarin.AndroidX.DrawerLayout.dll => 84
	i32 u0x4eed2679, ; 143: System.Linq => 132
	i32 u0x50255dd9, ; 144: lib-hr-Microsoft.Maui.Controls.resources.dll.so => 11
	i32 u0x50550fba, ; 145: Plugin.Maui.Audio => 59
	i32 u0x50acdfd7, ; 146: lib-ca-Microsoft.Maui.Controls.resources.dll.so => 1
	i32 u0x514fe3be, ; 147: Syncfusion.Maui.Gauges => 66
	i32 u0x52114ed3, ; 148: Xamarin.AndroidX.SavedState => 96
	i32 u0x52f8d494, ; 149: lib_Microsoft.Maui.Controls.Compatibility.dll.so => 50
	i32 u0x533678bd, ; 150: lib_System.Private.CoreLib.dll.so => 168
	i32 u0x53cefc50, ; 151: Xamarin.AndroidX.CoordinatorLayout => 80
	i32 u0x53f80ba6, ; 152: System.Runtime.Serialization.Formatters.dll => 153
	i32 u0x55ab7451, ; 153: Xamarin.AndroidX.Lifecycle.Common.Jvm => 86
	i32 u0x55e55df2, ; 154: Xamarin.AndroidX.Lifecycle.ViewModel.Android => 88
	i32 u0x568cd628, ; 155: System.Formats.Asn1.dll => 126
	i32 u0x56e7a7ad, ; 156: System.Net.Security.dll => 140
	i32 u0x5718a9ef, ; 157: System.Collections.Immutable.dll => 112
	i32 u0x57261233, ; 158: System.IO.Compression.dll => 128
	i32 u0x5732c11e, ; 159: InputKit.Maui => 39
	i32 u0x57924923, ; 160: Xamarin.AndroidX.AppCompat.AppCompatResources => 76
	i32 u0x57a5e912, ; 161: Microsoft.Extensions.Primitives => 49
	i32 u0x5833866d, ; 162: System.Collections.Immutable => 112
	i32 u0x583e844f, ; 163: System.IO.Compression.Brotli.dll => 127
	i32 u0x58fd6613, ; 164: hi/Microsoft.Maui.Controls.resources.dll => 10
	i32 u0x596b5b3a, ; 165: lib_System.Drawing.Primitives.dll.so => 124
	i32 u0x5a48cf6c, ; 166: el/Microsoft.Maui.Controls.resources.dll => 5
	i32 u0x5ae1cd96, ; 167: Plugin.LocalNotification => 58
	i32 u0x5baa5659, ; 168: Syncfusion.Maui.Buttons.dll => 62
	i32 u0x5be451c7, ; 169: lib_Xamarin.AndroidX.Browser.dll.so => 77
	i32 u0x5bf8ca0f, ; 170: System.Text.RegularExpressions.dll => 159
	i32 u0x5c7be408, ; 171: sk/Microsoft.Maui.Controls.resources.dll => 25
	i32 u0x5cabc9a4, ; 172: fr/Microsoft.Maui.Controls.resources => 8
	i32 u0x5da1ac3d, ; 173: lib_Syncfusion.Maui.Gauges.dll.so => 66
	i32 u0x5e0b6fdc, ; 174: Xamarin.KotlinX.Serialization.Core.Jvm.dll => 108
	i32 u0x5e33306d, ; 175: sv/Microsoft.Maui.Controls.resources => 26
	i32 u0x5e7321d2, ; 176: lib_System.ComponentModel.Primitives.dll.so => 116
	i32 u0x5ed5f779, ; 177: zh-Hant/Microsoft.Maui.Controls.resources => 33
	i32 u0x5f2ef0f8, ; 178: Syncfusion.Maui.Buttons => 62
	i32 u0x6078995d, ; 179: System.Net.WebSockets.Client.dll => 143
	i32 u0x60b0136a, ; 180: Xamarin.AndroidX.Loader.dll => 90
	i32 u0x60d97228, ; 181: Xamarin.AndroidX.ViewPager2 => 100
	i32 u0x61478ecc, ; 182: Microsoft.AspNet.SignalR.Client.dll => 40
	i32 u0x616edae3, ; 183: CommunityToolkit.Maui.Core.dll => 38
	i32 u0x6188ba7e, ; 184: Xamarin.AndroidX.CursorAdapter => 82
	i32 u0x61b9038d, ; 185: System.Net.Http.dll => 135
	i32 u0x61c036ca, ; 186: System.Text.RegularExpressions => 159
	i32 u0x62021776, ; 187: lib_System.IO.Compression.dll.so => 128
	i32 u0x620a8774, ; 188: lib_System.Xml.ReaderWriter.dll.so => 165
	i32 u0x62891830, ; 189: Syncfusion.Maui.Inputs.dll => 68
	i32 u0x62c6282e, ; 190: System.Runtime => 155
	i32 u0x62cec1a2, ; 191: lib_Xamarin.KotlinX.Coroutines.Core.Jvm.dll.so => 107
	i32 u0x62d6ea10, ; 192: Xamarin.Google.Android.Material.dll => 101
	i32 u0x63f46eb4, ; 193: lib_Syncfusion.Maui.Charts.dll.so => 63
	i32 u0x63fca3d0, ; 194: System.Net.Primitives.dll => 138
	i32 u0x640c0103, ; 195: System.Net.WebSockets => 144
	i32 u0x641f3e5a, ; 196: System.Security.Cryptography => 156
	i32 u0x66b3e15a, ; 197: lib_Microsoft.AspNet.SignalR.Client.dll.so => 40
	i32 u0x6715dc86, ; 198: Xamarin.AndroidX.CardView.dll => 78
	i32 u0x674b6e0a, ; 199: Syncfusion.Maui.TabView.dll => 73
	i32 u0x677cd287, ; 200: ro/Microsoft.Maui.Controls.resources.dll => 23
	i32 u0x68139a0d, ; 201: System.IO.Pipelines.dll => 129
	i32 u0x6816ab6a, ; 202: Mono.Android.Export => 170
	i32 u0x68f61ae4, ; 203: lib_System.Formats.Asn1.dll.so => 126
	i32 u0x690d4b7d, ; 204: lib-zh-Hant-Microsoft.Maui.Controls.resources.dll.so => 33
	i32 u0x69239124, ; 205: System.Diagnostics.TraceSource.dll => 123
	i32 u0x693efa35, ; 206: lib_System.Net.WebHeaderCollection.dll.so => 142
	i32 u0x6947f945, ; 207: Xamarin.AndroidX.SwipeRefreshLayout => 97
	i32 u0x6988f147, ; 208: Microsoft.Extensions.Logging.dll => 45
	i32 u0x69f4f41d, ; 209: lib_Xamarin.AndroidX.AppCompat.dll.so => 75
	i32 u0x6a216153, ; 210: Mono.Android.Runtime.dll => 171
	i32 u0x6a96652d, ; 211: Xamarin.AndroidX.Fragment => 85
	i32 u0x6afaf338, ; 212: lib_System.Threading.dll.so => 162
	i32 u0x6b645ada, ; 213: lib-fr-Microsoft.Maui.Controls.resources.dll.so => 8
	i32 u0x6bcd3296, ; 214: Xamarin.AndroidX.Loader => 90
	i32 u0x6be1e423, ; 215: nb/Microsoft.Maui.Controls.resources => 18
	i32 u0x6be29904, ; 216: lib_Xamarin.GooglePlayServices.Base.dll.so => 102
	i32 u0x6c111525, ; 217: Xamarin.Kotlin.StdLib.dll => 106
	i32 u0x6c13413e, ; 218: Xamarin.Google.Android.Material => 101
	i32 u0x6c652ce8, ; 219: Xamarin.AndroidX.Navigation.UI.dll => 94
	i32 u0x6c96614d, ; 220: hu/Microsoft.Maui.Controls.resources => 12
	i32 u0x6cc30c8c, ; 221: System.Runtime.Serialization.Formatters => 153
	i32 u0x6cf3d432, ; 222: lib_Xamarin.AndroidX.VersionedParcelable.dll.so => 98
	i32 u0x6cff90ba, ; 223: Microsoft.Extensions.Logging.Abstractions.dll => 46
	i32 u0x6dcaebf7, ; 224: uk/Microsoft.Maui.Controls.resources.dll => 29
	i32 u0x6ec71a65, ; 225: System.Linq.Expressions => 130
	i32 u0x7070c6c0, ; 226: lib-zh-Hans-Microsoft.Maui.Controls.resources.dll.so => 32
	i32 u0x71c62d98, ; 227: Xamarin.GooglePlayServices.Basement => 103
	i32 u0x71dc7c8b, ; 228: System.Collections.NonGeneric.dll => 113
	i32 u0x72fcebde, ; 229: lib_Xamarin.AndroidX.AppCompat.AppCompatResources.dll.so => 76
	i32 u0x731dd955, ; 230: lib_Mono.Android.dll.so => 172
	i32 u0x739bd4a8, ; 231: System.Private.Xml.Linq => 148
	i32 u0x73fbecbe, ; 232: lib_System.Memory.dll.so => 133
	i32 u0x74d743bf, ; 233: ja/Microsoft.Maui.Controls.resources => 15
	i32 u0x75533a5e, ; 234: Microsoft.Extensions.Configuration.dll => 41
	i32 u0x766c0c87, ; 235: lib_SFKBle_Admin.dll.so => 109
	i32 u0x781074ce, ; 236: hr/Microsoft.Maui.Controls.resources => 11
	i32 u0x78b622b1, ; 237: ar/Microsoft.Maui.Controls.resources.dll => 0
	i32 u0x7970be4f, ; 238: lib-he-Microsoft.Maui.Controls.resources.dll.so => 9
	i32 u0x79d00016, ; 239: it/Microsoft.Maui.Controls.resources => 14
	i32 u0x79eb68ee, ; 240: System.Private.Xml => 149
	i32 u0x7a80bd4e, ; 241: Xamarin.AndroidX.Lifecycle.LiveData.Core.dll => 87
	i32 u0x7b350579, ; 242: lib__Microsoft.Android.Resource.Designer.dll.so => 36
	i32 u0x7b6f419e, ; 243: System.Diagnostics.TraceSource => 123
	i32 u0x7bf8cdab, ; 244: System.Runtime.dll => 155
	i32 u0x7c9bf920, ; 245: System.Numerics.Vectors => 145
	i32 u0x7ec9ffe9, ; 246: System.Console => 119
	i32 u0x7eed5835, ; 247: Xamarin.GooglePlayServices.Base.dll => 102
	i32 u0x7f3ee2a0, ; 248: Syncfusion.Maui.Sliders.dll => 72
	i32 u0x7fb38cd2, ; 249: System.Collections.Specialized => 114
	i32 u0x7fdcdc37, ; 250: lib-ko-Microsoft.Maui.Controls.resources.dll.so => 16
	i32 u0x8030853e, ; 251: ko/Microsoft.Maui.Controls.resources => 16
	i32 u0x8044e1bd, ; 252: lib-ms-Microsoft.Maui.Controls.resources.dll.so => 17
	i32 u0x807f3512, ; 253: Syncfusion.Maui.Gauges.dll => 66
	i32 u0x80bd55ad, ; 254: Microsoft.Maui => 53
	i32 u0x80f2f56e, ; 255: lib_System.Runtime.Serialization.Formatters.dll.so => 153
	i32 u0x810c11c2, ; 256: ro/Microsoft.Maui.Controls.resources => 23
	i32 u0x816751d8, ; 257: lib_System.Diagnostics.DiagnosticSource.dll.so => 121
	i32 u0x82069348, ; 258: lib_InputKit.Maui.dll.so => 39
	i32 u0x820d22b3, ; 259: Microsoft.Extensions.Options.dll => 48
	i32 u0x82a8237c, ; 260: Microsoft.Extensions.Logging.Abstractions => 46
	i32 u0x82b6c85e, ; 261: System.ObjectModel.dll => 146
	i32 u0x82bb5429, ; 262: lib_System.Linq.Expressions.dll.so => 130
	i32 u0x83323b38, ; 263: Xamarin.KotlinX.Coroutines.Core.Jvm.dll => 107
	i32 u0x8334206b, ; 264: System.Net.Http => 135
	i32 u0x83633c5f, ; 265: lib-en-US-Syncfusion.Maui.Inputs.resources.dll.so => 35
	i32 u0x857e4dd2, ; 266: lib_System.Net.WebSockets.dll.so => 144
	i32 u0x8592bd9d, ; 267: Microsoft.AspNet.SignalR.Client => 40
	i32 u0x8628f1a4, ; 268: lib-ru-Microsoft.Maui.Controls.resources.dll.so => 24
	i32 u0x86bba59b, ; 269: lib_Microsoft.Maui.Controls.dll.so => 51
	i32 u0x871c9c1b, ; 270: Microsoft.Extensions.Configuration.Abstractions => 42
	i32 u0x875633cc, ; 271: fr/Microsoft.Maui.Controls.resources.dll => 8
	i32 u0x87a1a22b, ; 272: lib-it-Microsoft.Maui.Controls.resources.dll.so => 14
	i32 u0x87aabde6, ; 273: lib-en-US-Syncfusion.Maui.Buttons.resources.dll.so => 34
	i32 u0x87e25095, ; 274: Xamarin.AndroidX.RecyclerView.dll => 95
	i32 u0x87e7fdbb, ; 275: lib-nl-Microsoft.Maui.Controls.resources.dll.so => 19
	i32 u0x881f94da, ; 276: lib_netstandard.dll.so => 167
	i32 u0x8873eb17, ; 277: th/Microsoft.Maui.Controls.resources => 27
	i32 u0x88d8bfaa, ; 278: System.Net.Sockets => 141
	i32 u0x896b7878, ; 279: System.Private.CoreLib.dll => 168
	i32 u0x8a0cb154, ; 280: lib_Xamarin.GooglePlayServices.Location.dll.so => 104
	i32 u0x8b2f8b61, ; 281: lib_RGPopup.Maui.dll.so => 60
	i32 u0x8c20c628, ; 282: lib-fi-Microsoft.Maui.Controls.resources.dll.so => 7
	i32 u0x8c20f140, ; 283: lib_System.Console.dll.so => 119
	i32 u0x8c40e0db, ; 284: System.Net.Primitives => 138
	i32 u0x8c5a5413, ; 285: Syncfusion.Licensing => 61
	i32 u0x8d24e767, ; 286: System.Xml.ReaderWriter.dll => 165
	i32 u0x8d3fac99, ; 287: tr/Microsoft.Maui.Controls.resources => 28
	i32 u0x8d52b2e2, ; 288: Microsoft.Extensions.Configuration => 41
	i32 u0x8dcb0101, ; 289: lib_Xamarin.AndroidX.Navigation.Fragment.dll.so => 92
	i32 u0x8e02310f, ; 290: lib-ar-Microsoft.Maui.Controls.resources.dll.so => 0
	i32 u0x8f24faee, ; 291: System.Web.HttpUtility => 163
	i32 u0x8f8c64e2, ; 292: lib_System.Private.Xml.dll.so => 149
	i32 u0x905caa9d, ; 293: nl/Microsoft.Maui.Controls.resources => 19
	i32 u0x911615a7, ; 294: lib_Xamarin.AndroidX.Fragment.dll.so => 85
	i32 u0x912896e5, ; 295: System.Console.dll => 119
	i32 u0x91676790, ; 296: InputKit.Maui.dll => 39
	i32 u0x928c75ca, ; 297: System.Net.Sockets.dll => 141
	i32 u0x93554fdc, ; 298: netstandard.dll => 167
	i32 u0x93918882, ; 299: Java.Interop.dll => 169
	i32 u0x93dba8a1, ; 300: Microsoft.Maui.Controls => 51
	i32 u0x93fd4c66, ; 301: lib_Syncfusion.Maui.ProgressBar.dll.so => 71
	i32 u0x9438d78e, ; 302: lib_System.Text.Json.dll.so => 158
	i32 u0x94a1db18, ; 303: lib-id-Microsoft.Maui.Controls.resources.dll.so => 13
	i32 u0x9593ae7f, ; 304: lib_Xamarin.AndroidX.SavedState.dll.so => 96
	i32 u0x960aa9bc, ; 305: lib_Syncfusion.Maui.Sliders.dll.so => 72
	i32 u0x963ac2da, ; 306: sk/Microsoft.Maui.Controls.resources => 25
	i32 u0x96bea474, ; 307: lib_Microsoft.Maui.Controls.Xaml.dll.so => 52
	i32 u0x98964519, ; 308: RGPopup.Maui => 60
	i32 u0x98ba5a04, ; 309: Microsoft.CSharp => 110
	i32 u0x98e90c02, ; 310: lib_Xamarin.GooglePlayServices.Tasks.dll.so => 105
	i32 u0x9930ee42, ; 311: System.Text.Encodings.Web => 157
	i32 u0x9b24ab96, ; 312: lib_System.Runtime.Serialization.Primitives.dll.so => 154
	i32 u0x9b500441, ; 313: Xamarin.KotlinX.Coroutines.Core.Jvm => 107
	i32 u0x9bf052c1, ; 314: Microsoft.Extensions.Logging.Debug.dll => 47
	i32 u0x9bfe3a41, ; 315: System.Private.Xml.dll => 149
	i32 u0x9c375496, ; 316: Xamarin.AndroidX.CursorAdapter.dll => 82
	i32 u0x9c96ac4c, ; 317: lib_Xamarin.AndroidX.Navigation.UI.dll.so => 94
	i32 u0x9d4b58dd, ; 318: PINView.Maui.dll => 57
	i32 u0x9e78dac1, ; 319: lib_Xamarin.AndroidX.Lifecycle.ViewModelSavedState.dll.so => 89
	i32 u0x9ec4cf01, ; 320: System.Runtime.Loader => 151
	i32 u0x9ee22cc0, ; 321: System.Drawing.Primitives => 124
	i32 u0x9f7ea921, ; 322: lib_System.Runtime.InteropServices.dll.so => 150
	i32 u0x9f8c6f40, ; 323: System.Data.Common.dll => 120
	i32 u0xa0fb56af, ; 324: lib_System.Text.RegularExpressions.dll.so => 159
	i32 u0xa25c90e5, ; 325: lib_Xamarin.AndroidX.Core.dll.so => 81
	i32 u0xa262a30f, ; 326: System.Runtime.Numerics.dll => 152
	i32 u0xa2ce8457, ; 327: lib-es-Microsoft.Maui.Controls.resources.dll.so => 6
	i32 u0xa2e0939b, ; 328: Xamarin.AndroidX.Activity => 74
	i32 u0xa30769e5, ; 329: System.Threading.Channels => 160
	i32 u0xa32eb6f0, ; 330: Xamarin.AndroidX.AppCompat.AppCompatResources.dll => 76
	i32 u0xa3847b9d, ; 331: Syncfusion.Maui.DataSource => 65
	i32 u0xa3c818c7, ; 332: lib_System.Net.WebSockets.Client.dll.so => 143
	i32 u0xa4672f3b, ; 333: Microsoft.Maui.Controls.Xaml => 52
	i32 u0xa493aa02, ; 334: lib_System.Collections.Concurrent.dll.so => 111
	i32 u0xa4caf7a7, ; 335: Microsoft.Maui.dll => 53
	i32 u0xa4e79dfd, ; 336: Xamarin.AndroidX.Lifecycle.ViewModel.Android.dll => 88
	i32 u0xa5a0a402, ; 337: Xamarin.AndroidX.ViewPager.dll => 99
	i32 u0xa5b67c07, ; 338: Xamarin.AndroidX.Lifecycle.Common.Jvm.dll => 86
	i32 u0xa5c5753c, ; 339: lib_System.Collections.Immutable.dll.so => 112
	i32 u0xa668c988, ; 340: lib_System.Net.NameResolution.dll.so => 136
	i32 u0xa7008e0b, ; 341: Microsoft.Maui.Graphics => 55
	i32 u0xa7042ae3, ; 342: uk/Microsoft.Maui.Controls.resources => 29
	i32 u0xa741ef0b, ; 343: es/Microsoft.Maui.Controls.resources.dll => 6
	i32 u0xa744f665, ; 344: lib_Xamarin.AndroidX.Navigation.Runtime.dll.so => 93
	i32 u0xa78103bc, ; 345: Xamarin.AndroidX.CoordinatorLayout.dll => 80
	i32 u0xa7b5fe13, ; 346: lib_Syncfusion.Licensing.dll.so => 61
	i32 u0xa81b119f, ; 347: lib_System.Security.Cryptography.dll.so => 156
	i32 u0xa8c61dcb, ; 348: nl/Microsoft.Maui.Controls.resources.dll => 19
	i32 u0xa96bfab8, ; 349: Syncfusion.Maui.Sliders => 72
	i32 u0xa9b829f7, ; 350: Xamarin.GooglePlayServices.Base => 102
	i32 u0xaa107fc4, ; 351: Xamarin.AndroidX.ViewPager => 99
	i32 u0xaa4e51ff, ; 352: el/Microsoft.Maui.Controls.resources => 5
	i32 u0xaa88e550, ; 353: Mono.Android.Export.dll => 170
	i32 u0xaa8a4878, ; 354: Microsoft.Maui.Essentials => 54
	i32 u0xaaf9aad7, ; 355: CommunityToolkit.Maui.Core => 38
	i32 u0xaafab4cd, ; 356: Syncfusion.Licensing.dll => 61
	i32 u0xabbc23e8, ; 357: lib_Xamarin.KotlinX.Serialization.Core.Jvm.dll.so => 108
	i32 u0xabdea79a, ; 358: ru/Microsoft.Maui.Controls.resources => 24
	i32 u0xad6f1e8a, ; 359: System.Private.CoreLib => 168
	i32 u0xaddb6d38, ; 360: Xamarin.AndroidX.ViewPager2.dll => 100
	i32 u0xae037813, ; 361: System.Numerics.Vectors.dll => 145
	i32 u0xaeb2d8a5, ; 362: lib_Microsoft.Extensions.Options.dll.so => 48
	i32 u0xb0682092, ; 363: System.ComponentModel.dll => 118
	i32 u0xb18af942, ; 364: Xamarin.AndroidX.DrawerLayout => 84
	i32 u0xb1a434a2, ; 365: lib_System.Xml.Linq.dll.so => 164
	i32 u0xb223fa8c, ; 366: lib-cs-Microsoft.Maui.Controls.resources.dll.so => 2
	i32 u0xb514b305, ; 367: _Microsoft.Android.Resource.Designer.dll => 36
	i32 u0xb63fa9f0, ; 368: Xamarin.AndroidX.Navigation.Common => 91
	i32 u0xb646e70c, ; 369: Xamarin.GooglePlayServices.Tasks => 105
	i32 u0xb6490b5e, ; 370: lib_Mono.Android.Export.dll.so => 170
	i32 u0xb65adef9, ; 371: Mono.Android.Runtime => 171
	i32 u0xb660be12, ; 372: System.ComponentModel.Primitives => 116
	i32 u0xb6a153b2, ; 373: lib_Xamarin.AndroidX.ViewPager2.dll.so => 100
	i32 u0xb76be845, ; 374: hu/Microsoft.Maui.Controls.resources.dll => 12
	i32 u0xb8fd311b, ; 375: System.Formats.Asn1 => 126
	i32 u0xbaa520e7, ; 376: lib_System.ObjectModel.dll.so => 146
	i32 u0xbb3244c7, ; 377: Syncfusion.Maui.ListView => 69
	i32 u0xbb96e4f6, ; 378: Syncfusion.Maui.Core => 64
	i32 u0xbc98c93d, ; 379: lib_Xamarin.AndroidX.Collection.Jvm.dll.so => 79
	i32 u0xbcf2f620, ; 380: Syncfusion.Maui.ListView.dll => 69
	i32 u0xbd113355, ; 381: lib_Xamarin.AndroidX.Navigation.Common.dll.so => 91
	i32 u0xbd78b0c8, ; 382: Xamarin.AndroidX.Navigation.Fragment.dll => 92
	i32 u0xbea358c3, ; 383: SFKBle_Admin.dll => 109
	i32 u0xbff2e236, ; 384: System.Threading => 162
	i32 u0xc00e375b, ; 385: lib_Newtonsoft.Json.dll.so => 56
	i32 u0xc08d007e, ; 386: Xamarin.GooglePlayServices.Basement.dll => 103
	i32 u0xc1749a1c, ; 387: Syncfusion.Maui.Charts.dll => 63
	i32 u0xc235e84d, ; 388: Xamarin.AndroidX.CardView => 78
	i32 u0xc2a37b91, ; 389: System.Linq.Queryable.dll => 131
	i32 u0xc4030233, ; 390: lib_Syncfusion.Maui.Inputs.dll.so => 68
	i32 u0xc51c5eb9, ; 391: Syncfusion.Maui.Inputs => 68
	i32 u0xc591efe9, ; 392: lib_Microsoft.Extensions.Configuration.Abstractions.dll.so => 42
	i32 u0xc5b097e4, ; 393: System.Net.Requests.dll => 139
	i32 u0xc5b776df, ; 394: Xamarin.AndroidX.CustomView.dll => 83
	i32 u0xc774da4f, ; 395: Xamarin.AndroidX.Navigation.Runtime => 93
	i32 u0xc821fc10, ; 396: lib_System.ComponentModel.dll.so => 118
	i32 u0xc82afec1, ; 397: System.Text.Json => 158
	i32 u0xc86c06e3, ; 398: Xamarin.AndroidX.Core => 81
	i32 u0xc8a662e9, ; 399: Java.Interop => 169
	i32 u0xc8d10307, ; 400: lib_System.Diagnostics.TraceSource.dll.so => 123
	i32 u0xc92a6809, ; 401: Xamarin.AndroidX.RecyclerView => 95
	i32 u0xc9943a1e, ; 402: Syncfusion.Maui.GridCommon => 67
	i32 u0xcc5af6ee, ; 403: Microsoft.Extensions.DependencyInjection.dll => 43
	i32 u0xcc7d82b4, ; 404: netstandard => 167
	i32 u0xcda50957, ; 405: lib_Syncfusion.Maui.ListView.dll.so => 69
	i32 u0xcdc696e0, ; 406: Microsoft.Maui.Controls.Compatibility.dll => 50
	i32 u0xce3ccb24, ; 407: RGPopup.Maui.dll => 60
	i32 u0xce3fa116, ; 408: lib_System.Diagnostics.Process.dll.so => 122
	i32 u0xce70fda2, ; 409: hr/Microsoft.Maui.Controls.resources.dll => 11
	i32 u0xcef19b37, ; 410: System.ComponentModel.TypeConverter.dll => 117
	i32 u0xcf3163e6, ; 411: Mono.Android => 172
	i32 u0xcf663a21, ; 412: ru/Microsoft.Maui.Controls.resources.dll => 24
	i32 u0xcfa20c36, ; 413: lib_Xamarin.AndroidX.SwipeRefreshLayout.dll.so => 97
	i32 u0xcfbaacae, ; 414: System.Text.Json.dll => 158
	i32 u0xd0483fe8, ; 415: Xamarin.GooglePlayServices.Location.dll => 104
	i32 u0xd128d608, ; 416: System.Xml.Linq => 164
	i32 u0xd328ac54, ; 417: vi/Microsoft.Maui.Controls.resources => 30
	i32 u0xd4045e1b, ; 418: lib_System.dll.so => 166
	i32 u0xd4176e37, ; 419: Syncfusion.Maui.DataSource.dll => 65
	i32 u0xd457e5c9, ; 420: lib_Microsoft.CSharp.dll.so => 110
	i32 u0xd622b752, ; 421: lib-ro-Microsoft.Maui.Controls.resources.dll.so => 23
	i32 u0xd664cdf2, ; 422: de/Microsoft.Maui.Controls.resources => 4
	i32 u0xd67a52b3, ; 423: System.Net.WebSockets.Client => 143
	i32 u0xd715a361, ; 424: System.Linq.dll => 132
	i32 u0xd7f95f5a, ; 425: da/Microsoft.Maui.Controls.resources => 3
	i32 u0xd889aee8, ; 426: lib_System.Threading.Channels.dll.so => 160
	i32 u0xd8bba49d, ; 427: lib_Xamarin.AndroidX.RecyclerView.dll.so => 95
	i32 u0xd90e5f5a, ; 428: Xamarin.AndroidX.Lifecycle.LiveData.Core => 87
	i32 u0xd930cda0, ; 429: Xamarin.AndroidX.Navigation.Fragment => 92
	i32 u0xd96cf6f7, ; 430: pt-BR/Microsoft.Maui.Controls.resources => 21
	i32 u0xd9f65f5e, ; 431: lib-el-Microsoft.Maui.Controls.resources.dll.so => 5
	i32 u0xd9fdda56, ; 432: Microsoft.Extensions.Configuration.Abstractions.dll => 42
	i32 u0xda2f27df, ; 433: System.Net.NetworkInformation => 137
	i32 u0xda324084, ; 434: Plugin.Maui.Audio.dll => 59
	i32 u0xda4773dd, ; 435: he/Microsoft.Maui.Controls.resources => 9
	i32 u0xdae8aa5e, ; 436: Mono.Android.dll => 172
	i32 u0xdb7f7e5d, ; 437: Xamarin.AndroidX.Browser => 77
	i32 u0xdbb50d93, ; 438: ms/Microsoft.Maui.Controls.resources => 17
	i32 u0xdc5370c5, ; 439: lib_System.Web.HttpUtility.dll.so => 163
	i32 u0xdc68940c, ; 440: zh-Hant/Microsoft.Maui.Controls.resources.dll => 33
	i32 u0xde068c70, ; 441: Xamarin.AndroidX.Navigation.Common.dll => 91
	i32 u0xde7354ab, ; 442: System.Net.NameResolution => 136
	i32 u0xdecad304, ; 443: System.Net.Http.Json.dll => 134
	i32 u0xdf6f3870, ; 444: System.Diagnostics.DiagnosticSource => 121
	i32 u0xe004dc18, ; 445: lib_Syncfusion.Maui.GridCommon.dll.so => 67
	i32 u0xe0dec2ee, ; 446: lib_CommunityToolkit.Maui.dll.so => 37
	i32 u0xe13414bb, ; 447: lib-hu-Microsoft.Maui.Controls.resources.dll.so => 12
	i32 u0xe1f0a5d8, ; 448: lib_Xamarin.AndroidX.ViewPager.dll.so => 99
	i32 u0xe2098b0b, ; 449: System.Collections.NonGeneric => 113
	i32 u0xe250cda6, ; 450: lib_Microsoft.Extensions.Logging.dll.so => 45
	i32 u0xe2513246, ; 451: lib_System.Runtime.Numerics.dll.so => 152
	i32 u0xe28e5915, ; 452: Microsoft.Maui.Controls.Compatibility => 50
	i32 u0xe2a3f2e8, ; 453: System.Collections.Specialized.dll => 114
	i32 u0xe34ee011, ; 454: lib_System.IO.Pipelines.dll.so => 129
	i32 u0xe3886bf7, ; 455: CommunityToolkit.Maui.dll => 37
	i32 u0xe3df9d2b, ; 456: System.Security.Cryptography.dll => 156
	i32 u0xe4fab729, ; 457: Microsoft.Extensions.DependencyInjection.Abstractions.dll => 44
	i32 u0xe53f2dd7, ; 458: en-US/Syncfusion.Maui.Inputs.resources => 35
	i32 u0xe56ef253, ; 459: System.Runtime.InteropServices.dll => 150
	i32 u0xe625b819, ; 460: lib_Xamarin.AndroidX.CardView.dll.so => 78
	i32 u0xe797fcc1, ; 461: System.Net.WebHeaderCollection.dll => 142
	i32 u0xe7dc15ff, ; 462: zh-Hans/Microsoft.Maui.Controls.resources.dll => 32
	i32 u0xe839deed, ; 463: System.Collections.Concurrent.dll => 111
	i32 u0xe843daa0, ; 464: Xamarin.AndroidX.Core.dll => 81
	i32 u0xe90fdb70, ; 465: Xamarin.AndroidX.Collection.Jvm => 79
	i32 u0xe99f7d24, ; 466: lib-tr-Microsoft.Maui.Controls.resources.dll.so => 28
	i32 u0xe9b630ed, ; 467: Xamarin.AndroidX.VersionedParcelable.dll => 98
	i32 u0xea213423, ; 468: System.Xml.ReaderWriter => 165
	i32 u0xea4fb52e, ; 469: Xamarin.AndroidX.Navigation.UI => 94
	i32 u0xeab81858, ; 470: lib_Microsoft.Maui.Essentials.dll.so => 54
	i32 u0xeaf598f6, ; 471: lib_Microsoft.Extensions.Logging.Abstractions.dll.so => 46
	i32 u0xebb0254b, ; 472: lib_System.Net.NetworkInformation.dll.so => 137
	i32 u0xebc66336, ; 473: Xamarin.AndroidX.AppCompat.dll => 75
	i32 u0xec7623e9, ; 474: Xamarin.GooglePlayServices.Location => 104
	i32 u0xeca1adaf, ; 475: Xamarin.GooglePlayServices.Tasks.dll => 105
	i32 u0xed1090ae, ; 476: lib_System.Net.Primitives.dll.so => 138
	i32 u0xed409aea, ; 477: th/Microsoft.Maui.Controls.resources.dll => 27
	i32 u0xed6137e5, ; 478: en-US/Syncfusion.Maui.Inputs.resources.dll => 35
	i32 u0xed96d41f, ; 479: lib_Xamarin.AndroidX.CoordinatorLayout.dll.so => 80
	i32 u0xedadd6e2, ; 480: he/Microsoft.Maui.Controls.resources.dll => 9
	i32 u0xedf6669b, ; 481: lib_System.Drawing.dll.so => 125
	i32 u0xee9f991d, ; 482: System.Diagnostics.Process.dll => 122
	i32 u0xefd01a89, ; 483: System.IO.Pipelines => 129
	i32 u0xeff49a63, ; 484: System.Memory => 133
	i32 u0xf0672b49, ; 485: lib_Syncfusion.Maui.Core.dll.so => 64
	i32 u0xf121f953, ; 486: lib_Xamarin.AndroidX.Lifecycle.LiveData.Core.dll.so => 87
	i32 u0xf1304331, ; 487: Microsoft.Maui.Controls.Xaml.dll => 52
	i32 u0xf1676aaa, ; 488: lib-da-Microsoft.Maui.Controls.resources.dll.so => 3
	i32 u0xf27f60d1, ; 489: System.Private.Xml.Linq.dll => 148
	i32 u0xf29c5384, ; 490: id/Microsoft.Maui.Controls.resources => 13
	i32 u0xf2ce3c98, ; 491: System.Threading.dll => 162
	i32 u0xf2dd3fc4, ; 492: lib-ja-Microsoft.Maui.Controls.resources.dll.so => 15
	i32 u0xf323e0a6, ; 493: lib_Xamarin.Kotlin.StdLib.dll.so => 106
	i32 u0xf40add04, ; 494: Microsoft.Maui.Essentials.dll => 54
	i32 u0xf45985cf, ; 495: System.Drawing.dll => 125
	i32 u0xf462c30d, ; 496: System.Private.Uri => 147
	i32 u0xf48143e5, ; 497: pt/Microsoft.Maui.Controls.resources.dll => 22
	i32 u0xf5185c24, ; 498: lib-pt-Microsoft.Maui.Controls.resources.dll.so => 22
	i32 u0xf53cb11d, ; 499: lib_System.Net.Security.dll.so => 140
	i32 u0xf5861a4f, ; 500: pl/Microsoft.Maui.Controls.resources => 20
	i32 u0xf5e94e90, ; 501: ms/Microsoft.Maui.Controls.resources.dll => 17
	i32 u0xf5f4f1f0, ; 502: Microsoft.Extensions.DependencyInjection => 43
	i32 u0xf5fdf056, ; 503: lib_Microsoft.Extensions.DependencyInjection.dll.so => 43
	i32 u0xf84b5f26, ; 504: lib_Syncfusion.Maui.TabView.dll.so => 73
	i32 u0xf86129d4, ; 505: lib-sv-Microsoft.Maui.Controls.resources.dll.so => 26
	i32 u0xf93ba7d4, ; 506: System.Runtime.Serialization.Primitives => 154
	i32 u0xf94a8f86, ; 507: Xamarin.AndroidX.Lifecycle.ViewModelSavedState.dll => 89
	i32 u0xf9bf7028, ; 508: SFKBle_Admin => 109
	i32 u0xfa50891f, ; 509: lib_System.Linq.dll.so => 132
	i32 u0xfb0af295, ; 510: lib-zh-HK-Microsoft.Maui.Controls.resources.dll.so => 31
	i32 u0xfb1dad5d, ; 511: System.Diagnostics.DiagnosticSource.dll => 121
	i32 u0xfbc4b67c, ; 512: lib_System.IO.Compression.Brotli.dll.so => 127
	i32 u0xfc5f7d36, ; 513: pt/Microsoft.Maui.Controls.resources => 22
	i32 u0xfd4f3a26, ; 514: lib_Syncfusion.Maui.Buttons.dll.so => 62
	i32 u0xfea12dee, ; 515: Microsoft.Maui.Controls.dll => 51
	i32 u0xfecef6ea, ; 516: System.Runtime.Numerics => 152
	i32 u0xff616445, ; 517: lib_Syncfusion.Maui.DataSource.dll.so => 65
	i32 u0xffd4917f ; 518: Xamarin.AndroidX.Lifecycle.ViewModelSavedState => 89
], align 4

@assembly_image_cache_indices = dso_local local_unnamed_addr constant [519 x i32] [
	i32 137, i32 139, i32 136, i32 90, i32 44, i32 70, i32 56, i32 144,
	i32 29, i32 161, i32 53, i32 1, i32 34, i32 55, i32 113, i32 131,
	i32 31, i32 150, i32 160, i32 142, i32 74, i32 64, i32 97, i32 30,
	i32 101, i32 134, i32 20, i32 30, i32 31, i32 118, i32 77, i32 47,
	i32 110, i32 83, i32 18, i32 58, i32 63, i32 2, i32 25, i32 57,
	i32 74, i32 155, i32 15, i32 14, i32 2, i32 41, i32 151, i32 70,
	i32 34, i32 161, i32 133, i32 36, i32 26, i32 115, i32 85, i32 163,
	i32 67, i32 171, i32 154, i32 169, i32 166, i32 147, i32 146, i32 13,
	i32 7, i32 58, i32 49, i32 10, i32 131, i32 45, i32 71, i32 21,
	i32 114, i32 37, i32 141, i32 71, i32 83, i32 4, i32 38, i32 157,
	i32 98, i32 120, i32 111, i32 21, i32 140, i32 1, i32 164, i32 82,
	i32 70, i32 16, i32 4, i32 151, i32 59, i32 139, i32 128, i32 48,
	i32 120, i32 161, i32 147, i32 127, i32 84, i32 0, i32 134, i32 116,
	i32 103, i32 28, i32 88, i32 86, i32 56, i32 115, i32 32, i32 6,
	i32 96, i32 125, i32 44, i32 3, i32 75, i32 124, i32 130, i32 115,
	i32 49, i32 117, i32 106, i32 166, i32 157, i32 47, i32 27, i32 117,
	i32 145, i32 135, i32 73, i32 93, i32 7, i32 20, i32 122, i32 18,
	i32 10, i32 79, i32 57, i32 55, i32 108, i32 148, i32 84, i32 132,
	i32 11, i32 59, i32 1, i32 66, i32 96, i32 50, i32 168, i32 80,
	i32 153, i32 86, i32 88, i32 126, i32 140, i32 112, i32 128, i32 39,
	i32 76, i32 49, i32 112, i32 127, i32 10, i32 124, i32 5, i32 58,
	i32 62, i32 77, i32 159, i32 25, i32 8, i32 66, i32 108, i32 26,
	i32 116, i32 33, i32 62, i32 143, i32 90, i32 100, i32 40, i32 38,
	i32 82, i32 135, i32 159, i32 128, i32 165, i32 68, i32 155, i32 107,
	i32 101, i32 63, i32 138, i32 144, i32 156, i32 40, i32 78, i32 73,
	i32 23, i32 129, i32 170, i32 126, i32 33, i32 123, i32 142, i32 97,
	i32 45, i32 75, i32 171, i32 85, i32 162, i32 8, i32 90, i32 18,
	i32 102, i32 106, i32 101, i32 94, i32 12, i32 153, i32 98, i32 46,
	i32 29, i32 130, i32 32, i32 103, i32 113, i32 76, i32 172, i32 148,
	i32 133, i32 15, i32 41, i32 109, i32 11, i32 0, i32 9, i32 14,
	i32 149, i32 87, i32 36, i32 123, i32 155, i32 145, i32 119, i32 102,
	i32 72, i32 114, i32 16, i32 16, i32 17, i32 66, i32 53, i32 153,
	i32 23, i32 121, i32 39, i32 48, i32 46, i32 146, i32 130, i32 107,
	i32 135, i32 35, i32 144, i32 40, i32 24, i32 51, i32 42, i32 8,
	i32 14, i32 34, i32 95, i32 19, i32 167, i32 27, i32 141, i32 168,
	i32 104, i32 60, i32 7, i32 119, i32 138, i32 61, i32 165, i32 28,
	i32 41, i32 92, i32 0, i32 163, i32 149, i32 19, i32 85, i32 119,
	i32 39, i32 141, i32 167, i32 169, i32 51, i32 71, i32 158, i32 13,
	i32 96, i32 72, i32 25, i32 52, i32 60, i32 110, i32 105, i32 157,
	i32 154, i32 107, i32 47, i32 149, i32 82, i32 94, i32 57, i32 89,
	i32 151, i32 124, i32 150, i32 120, i32 159, i32 81, i32 152, i32 6,
	i32 74, i32 160, i32 76, i32 65, i32 143, i32 52, i32 111, i32 53,
	i32 88, i32 99, i32 86, i32 112, i32 136, i32 55, i32 29, i32 6,
	i32 93, i32 80, i32 61, i32 156, i32 19, i32 72, i32 102, i32 99,
	i32 5, i32 170, i32 54, i32 38, i32 61, i32 108, i32 24, i32 168,
	i32 100, i32 145, i32 48, i32 118, i32 84, i32 164, i32 2, i32 36,
	i32 91, i32 105, i32 170, i32 171, i32 116, i32 100, i32 12, i32 126,
	i32 146, i32 69, i32 64, i32 79, i32 69, i32 91, i32 92, i32 109,
	i32 162, i32 56, i32 103, i32 63, i32 78, i32 131, i32 68, i32 68,
	i32 42, i32 139, i32 83, i32 93, i32 118, i32 158, i32 81, i32 169,
	i32 123, i32 95, i32 67, i32 43, i32 167, i32 69, i32 50, i32 60,
	i32 122, i32 11, i32 117, i32 172, i32 24, i32 97, i32 158, i32 104,
	i32 164, i32 30, i32 166, i32 65, i32 110, i32 23, i32 4, i32 143,
	i32 132, i32 3, i32 160, i32 95, i32 87, i32 92, i32 21, i32 5,
	i32 42, i32 137, i32 59, i32 9, i32 172, i32 77, i32 17, i32 163,
	i32 33, i32 91, i32 136, i32 134, i32 121, i32 67, i32 37, i32 12,
	i32 99, i32 113, i32 45, i32 152, i32 50, i32 114, i32 129, i32 37,
	i32 156, i32 44, i32 35, i32 150, i32 78, i32 142, i32 32, i32 111,
	i32 81, i32 79, i32 28, i32 98, i32 165, i32 94, i32 54, i32 46,
	i32 137, i32 75, i32 104, i32 105, i32 138, i32 27, i32 35, i32 80,
	i32 9, i32 125, i32 122, i32 129, i32 133, i32 64, i32 87, i32 52,
	i32 3, i32 148, i32 13, i32 162, i32 15, i32 106, i32 54, i32 125,
	i32 147, i32 22, i32 22, i32 140, i32 20, i32 17, i32 43, i32 43,
	i32 73, i32 26, i32 154, i32 89, i32 109, i32 132, i32 31, i32 121,
	i32 127, i32 22, i32 62, i32 51, i32 152, i32 65, i32 89
], align 4

@marshal_methods_number_of_classes = dso_local local_unnamed_addr constant i32 0, align 4

@marshal_methods_class_cache = dso_local local_unnamed_addr global [0 x %struct.MarshalMethodsManagedClass] zeroinitializer, align 4

; Names of classes in which marshal methods reside
@mm_class_names = dso_local local_unnamed_addr constant [0 x ptr] zeroinitializer, align 4

@mm_method_names = dso_local local_unnamed_addr constant [1 x %struct.MarshalMethodName] [
	%struct.MarshalMethodName {
		i64 u0x0000000000000000, ; name: 
		ptr @.MarshalMethodName.0_name; char* name
	} ; 0
], align 8

; get_function_pointer (uint32_t mono_image_index, uint32_t class_index, uint32_t method_token, void*& target_ptr)
@get_function_pointer = internal dso_local unnamed_addr global ptr null, align 4

; Functions

; Function attributes: memory(write, argmem: none, inaccessiblemem: none) "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" uwtable willreturn
define void @xamarin_app_init(ptr nocapture noundef readnone %env, ptr noundef %fn) local_unnamed_addr #0
{
	%fnIsNull = icmp eq ptr %fn, null
	br i1 %fnIsNull, label %1, label %2

1: ; preds = %0
	%putsResult = call noundef i32 @puts(ptr @.str.0)
	call void @abort()
	unreachable 

2: ; preds = %1, %0
	store ptr %fn, ptr @get_function_pointer, align 4, !tbaa !3
	ret void
}

; Strings
@.str.0 = private unnamed_addr constant [40 x i8] c"get_function_pointer MUST be specified\0A\00", align 1

;MarshalMethodName
@.MarshalMethodName.0_name = private unnamed_addr constant [1 x i8] c"\00", align 1

; External functions

; Function attributes: noreturn "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8"
declare void @abort() local_unnamed_addr #2

; Function attributes: nofree nounwind
declare noundef i32 @puts(ptr noundef) local_unnamed_addr #1
attributes #0 = { memory(write, argmem: none, inaccessiblemem: none) "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" "target-cpu"="generic" "target-features"="+armv7-a,+d32,+dsp,+fp64,+neon,+vfp2,+vfp2sp,+vfp3,+vfp3d16,+vfp3d16sp,+vfp3sp,-aes,-fp-armv8,-fp-armv8d16,-fp-armv8d16sp,-fp-armv8sp,-fp16,-fp16fml,-fullfp16,-sha2,-thumb-mode,-vfp4,-vfp4d16,-vfp4d16sp,-vfp4sp" uwtable willreturn }
attributes #1 = { nofree nounwind }
attributes #2 = { noreturn "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" "target-cpu"="generic" "target-features"="+armv7-a,+d32,+dsp,+fp64,+neon,+vfp2,+vfp2sp,+vfp3,+vfp3d16,+vfp3d16sp,+vfp3sp,-aes,-fp-armv8,-fp-armv8d16,-fp-armv8d16sp,-fp-armv8sp,-fp16,-fp16fml,-fullfp16,-sha2,-thumb-mode,-vfp4,-vfp4d16,-vfp4d16sp,-vfp4sp" }

; Metadata
!llvm.module.flags = !{!0, !1, !7}
!0 = !{i32 1, !"wchar_size", i32 4}
!1 = !{i32 7, !"PIC Level", i32 2}
!llvm.ident = !{!2}
!2 = !{!".NET for Android remotes/origin/release/9.0.1xx @ 1dcfb6f8779c33b6f768c996495cb90ecd729329"}
!3 = !{!4, !4, i64 0}
!4 = !{!"any pointer", !5, i64 0}
!5 = !{!"omnipotent char", !6, i64 0}
!6 = !{!"Simple C++ TBAA"}
!7 = !{i32 1, !"min_enum_size", i32 4}
