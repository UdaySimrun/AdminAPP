; ModuleID = 'marshal_methods.arm64-v8a.ll'
source_filename = "marshal_methods.arm64-v8a.ll"
target datalayout = "e-m:e-i8:8:32-i16:16:32-i64:64-i128:128-n32:64-S128"
target triple = "aarch64-unknown-linux-android21"

%struct.MarshalMethodName = type {
	i64, ; uint64_t id
	ptr ; char* name
}

%struct.MarshalMethodsManagedClass = type {
	i32, ; uint32_t token
	ptr ; MonoClass klass
}

@assembly_image_cache = dso_local local_unnamed_addr global [173 x ptr] zeroinitializer, align 8

; Each entry maps hash of an assembly name to an index into the `assembly_image_cache` array
@assembly_image_cache_hashes = dso_local local_unnamed_addr constant [519 x i64] [
	i64 u0x0071cf2d27b7d61e, ; 0: lib_Xamarin.AndroidX.SwipeRefreshLayout.dll.so => 97
	i64 u0x02123411c4e01926, ; 1: lib_Xamarin.AndroidX.Navigation.Runtime.dll.so => 93
	i64 u0x022e81ea9c46e03a, ; 2: lib_CommunityToolkit.Maui.Core.dll.so => 38
	i64 u0x02abedc11addc1ed, ; 3: lib_Mono.Android.Runtime.dll.so => 171
	i64 u0x032267b2a94db371, ; 4: lib_Xamarin.AndroidX.AppCompat.dll.so => 75
	i64 u0x03ed5b0d8b13b9ff, ; 5: RGPopup.Maui => 60
	i64 u0x043032f1d071fae0, ; 6: ru/Microsoft.Maui.Controls.resources => 24
	i64 u0x044440a55165631e, ; 7: lib-cs-Microsoft.Maui.Controls.resources.dll.so => 2
	i64 u0x046eb1581a80c6b0, ; 8: vi/Microsoft.Maui.Controls.resources => 30
	i64 u0x0517ef04e06e9f76, ; 9: System.Net.Primitives => 138
	i64 u0x051a3be159e4ef99, ; 10: Xamarin.GooglePlayServices.Tasks => 105
	i64 u0x0565d18c6da3de38, ; 11: Xamarin.AndroidX.RecyclerView => 95
	i64 u0x0581db89237110e9, ; 12: lib_System.Collections.dll.so => 115
	i64 u0x05989cb940b225a9, ; 13: Microsoft.Maui.dll => 53
	i64 u0x06076b5d2b581f08, ; 14: zh-HK/Microsoft.Maui.Controls.resources => 31
	i64 u0x06388ffe9f6c161a, ; 15: System.Xml.Linq.dll => 164
	i64 u0x0680a433c781bb3d, ; 16: Xamarin.AndroidX.Collection.Jvm => 79
	i64 u0x07c57877c7ba78ad, ; 17: ru/Microsoft.Maui.Controls.resources.dll => 24
	i64 u0x07dcdc7460a0c5e4, ; 18: System.Collections.NonGeneric => 113
	i64 u0x08122e52765333c8, ; 19: lib_Microsoft.Extensions.Logging.Debug.dll.so => 47
	i64 u0x08f3c9788ee2153c, ; 20: Xamarin.AndroidX.DrawerLayout => 84
	i64 u0x0919c28b89381a0b, ; 21: lib_Microsoft.Extensions.Options.dll.so => 48
	i64 u0x092266563089ae3e, ; 22: lib_System.Collections.NonGeneric.dll.so => 113
	i64 u0x098b50f911ccea8d, ; 23: lib_Xamarin.GooglePlayServices.Basement.dll.so => 103
	i64 u0x09d144a7e214d457, ; 24: System.Security.Cryptography => 156
	i64 u0x0aadafcd1a44fbfa, ; 25: Syncfusion.Maui.TabView.dll => 73
	i64 u0x0abb3e2b271edc45, ; 26: System.Threading.Channels.dll => 160
	i64 u0x0b3b632c3bbee20c, ; 27: sk/Microsoft.Maui.Controls.resources => 25
	i64 u0x0b6aff547b84fbe9, ; 28: Xamarin.KotlinX.Serialization.Core.Jvm => 108
	i64 u0x0be2e1f8ce4064ed, ; 29: Xamarin.AndroidX.ViewPager => 99
	i64 u0x0c3ca6cc978e2aae, ; 30: pt-BR/Microsoft.Maui.Controls.resources => 21
	i64 u0x0c59ad9fbbd43abe, ; 31: Mono.Android => 172
	i64 u0x0c7790f60165fc06, ; 32: lib_Microsoft.Maui.Essentials.dll.so => 54
	i64 u0x0e14e73a54dda68e, ; 33: lib_System.Net.NameResolution.dll.so => 136
	i64 u0x102a31b45304b1da, ; 34: Xamarin.AndroidX.CustomView => 83
	i64 u0x10ca46a12d1cfb88, ; 35: Syncfusion.Maui.Core => 64
	i64 u0x10f6cfcbcf801616, ; 36: System.IO.Compression.Brotli => 127
	i64 u0x1176f12a4db52a13, ; 37: Syncfusion.Maui.Charts.dll => 63
	i64 u0x11a70d0e1009fb11, ; 38: System.Net.WebSockets.dll => 144
	i64 u0x125b7f94acb989db, ; 39: Xamarin.AndroidX.RecyclerView.dll => 95
	i64 u0x12ac84baeddc8a51, ; 40: lib_Syncfusion.Maui.Gauges.dll.so => 66
	i64 u0x138567fa954faa55, ; 41: Xamarin.AndroidX.Browser => 77
	i64 u0x13a01de0cbc3f06c, ; 42: lib-fr-Microsoft.Maui.Controls.resources.dll.so => 8
	i64 u0x13f1e5e209e91af4, ; 43: lib_Java.Interop.dll.so => 169
	i64 u0x13f1e880c25d96d1, ; 44: he/Microsoft.Maui.Controls.resources => 9
	i64 u0x143d8ea60a6a4011, ; 45: Microsoft.Extensions.DependencyInjection.Abstractions => 44
	i64 u0x1497051b917530bd, ; 46: lib_System.Net.WebSockets.dll.so => 144
	i64 u0x1695ecefb732cade, ; 47: lib_Syncfusion.Maui.Core.dll.so => 64
	i64 u0x17125c9a85b4929f, ; 48: lib_netstandard.dll.so => 167
	i64 u0x17b56e25558a5d36, ; 49: lib-hu-Microsoft.Maui.Controls.resources.dll.so => 12
	i64 u0x17f9358913beb16a, ; 50: System.Text.Encodings.Web => 157
	i64 u0x18402a709e357f3b, ; 51: lib_Xamarin.KotlinX.Serialization.Core.Jvm.dll.so => 108
	i64 u0x18d60a6bac8b35e4, ; 52: Syncfusion.Maui.GridCommon.dll => 67
	i64 u0x18f0ce884e87d89a, ; 53: nb/Microsoft.Maui.Controls.resources.dll => 18
	i64 u0x193d2791510c1857, ; 54: Syncfusion.Maui.Gauges.dll => 66
	i64 u0x1a91866a319e9259, ; 55: lib_System.Collections.Concurrent.dll.so => 111
	i64 u0x1aac34d1917ba5d3, ; 56: lib_System.dll.so => 166
	i64 u0x1aad60783ffa3e5b, ; 57: lib-th-Microsoft.Maui.Controls.resources.dll.so => 27
	i64 u0x1c753b5ff15bce1b, ; 58: Mono.Android.Runtime.dll => 171
	i64 u0x1d4c109ca6e27ed8, ; 59: lib_Microsoft.Maui.Controls.Compatibility.dll.so => 50
	i64 u0x1e3d87657e9659bc, ; 60: Xamarin.AndroidX.Navigation.UI => 94
	i64 u0x1e71143913d56c10, ; 61: lib-ko-Microsoft.Maui.Controls.resources.dll.so => 16
	i64 u0x1ed8fcce5e9b50a0, ; 62: Microsoft.Extensions.Options.dll => 48
	i64 u0x209375905fcc1bad, ; 63: lib_System.IO.Compression.Brotli.dll.so => 127
	i64 u0x20fab3cf2dfbc8df, ; 64: lib_System.Diagnostics.Process.dll.so => 122
	i64 u0x2174319c0d835bc9, ; 65: System.Runtime => 155
	i64 u0x21ef8aede7dcbb79, ; 66: Syncfusion.Maui.Buttons.dll => 62
	i64 u0x220fd4f2e7c48170, ; 67: th/Microsoft.Maui.Controls.resources => 27
	i64 u0x2347c268e3e4e536, ; 68: Xamarin.GooglePlayServices.Basement.dll => 103
	i64 u0x237be844f1f812c7, ; 69: System.Threading.Thread.dll => 161
	i64 u0x2407aef2bbe8fadf, ; 70: System.Console => 119
	i64 u0x240abe014b27e7d3, ; 71: Xamarin.AndroidX.Core.dll => 81
	i64 u0x247619fe4413f8bf, ; 72: System.Runtime.Serialization.Primitives.dll => 154
	i64 u0x252073cc3caa62c2, ; 73: fr/Microsoft.Maui.Controls.resources.dll => 8
	i64 u0x2662c629b96b0b30, ; 74: lib_Xamarin.Kotlin.StdLib.dll.so => 106
	i64 u0x268c1439f13bcc29, ; 75: lib_Microsoft.Extensions.Primitives.dll.so => 49
	i64 u0x273f3515de5faf0d, ; 76: id/Microsoft.Maui.Controls.resources.dll => 13
	i64 u0x2742545f9094896d, ; 77: hr/Microsoft.Maui.Controls.resources => 11
	i64 u0x2759af78ab94d39b, ; 78: System.Net.WebSockets => 144
	i64 u0x27b410442fad6cf1, ; 79: Java.Interop.dll => 169
	i64 u0x2801845a2c71fbfb, ; 80: System.Net.Primitives.dll => 138
	i64 u0x28e491b4ae3aff19, ; 81: Microsoft.AspNet.SignalR.Client.dll => 40
	i64 u0x2a128783efe70ba0, ; 82: uk/Microsoft.Maui.Controls.resources.dll => 29
	i64 u0x2a32a01be82d61e4, ; 83: lib_Microsoft.AspNet.SignalR.Client.dll.so => 40
	i64 u0x2a3b095612184159, ; 84: lib_System.Net.NetworkInformation.dll.so => 137
	i64 u0x2a6507a5ffabdf28, ; 85: System.Diagnostics.TraceSource.dll => 123
	i64 u0x2ad156c8e1354139, ; 86: fi/Microsoft.Maui.Controls.resources => 7
	i64 u0x2af298f63581d886, ; 87: System.Text.RegularExpressions.dll => 159
	i64 u0x2afc1c4f898552ee, ; 88: lib_System.Formats.Asn1.dll.so => 126
	i64 u0x2b148910ed40fbf9, ; 89: zh-Hant/Microsoft.Maui.Controls.resources.dll => 33
	i64 u0x2c8bd14bb93a7d82, ; 90: lib-pl-Microsoft.Maui.Controls.resources.dll.so => 20
	i64 u0x2cd723e9fe623c7c, ; 91: lib_System.Private.Xml.Linq.dll.so => 148
	i64 u0x2cdbe1c1d4183ec1, ; 92: lib_Syncfusion.Licensing.dll.so => 61
	i64 u0x2d169d318a968379, ; 93: System.Threading.dll => 162
	i64 u0x2d47774b7d993f59, ; 94: sv/Microsoft.Maui.Controls.resources.dll => 26
	i64 u0x2db915caf23548d2, ; 95: System.Text.Json.dll => 158
	i64 u0x2e6f1f226821322a, ; 96: el/Microsoft.Maui.Controls.resources.dll => 5
	i64 u0x2f02f94df3200fe5, ; 97: System.Diagnostics.Process => 122
	i64 u0x2f2e98e1c89b1aff, ; 98: System.Xml.ReaderWriter => 165
	i64 u0x309ee9eeec09a71e, ; 99: lib_Xamarin.AndroidX.Fragment.dll.so => 85
	i64 u0x31195fef5d8fb552, ; 100: _Microsoft.Android.Resource.Designer.dll => 36
	i64 u0x32243413e774362a, ; 101: Xamarin.AndroidX.CardView.dll => 78
	i64 u0x3235427f8d12dae1, ; 102: lib_System.Drawing.Primitives.dll.so => 124
	i64 u0x329753a17a517811, ; 103: fr/Microsoft.Maui.Controls.resources => 8
	i64 u0x32aa989ff07a84ff, ; 104: lib_System.Xml.ReaderWriter.dll.so => 165
	i64 u0x33829542f112d59b, ; 105: System.Collections.Immutable => 112
	i64 u0x33a31443733849fe, ; 106: lib-es-Microsoft.Maui.Controls.resources.dll.so => 6
	i64 u0x33ce9e565c8c9748, ; 107: Syncfusion.Maui.ProgressBar.dll => 71
	i64 u0x341abc357fbb4ebf, ; 108: lib_System.Net.Sockets.dll.so => 141
	i64 u0x34c492cef793bb77, ; 109: lib_InputKit.Maui.dll.so => 39
	i64 u0x34dfd74fe2afcf37, ; 110: Microsoft.Maui => 53
	i64 u0x34e292762d9615df, ; 111: cs/Microsoft.Maui.Controls.resources.dll => 2
	i64 u0x3508234247f48404, ; 112: Microsoft.Maui.Controls => 51
	i64 u0x3549870798b4cd30, ; 113: lib_Xamarin.AndroidX.ViewPager2.dll.so => 100
	i64 u0x355282fc1c909694, ; 114: Microsoft.Extensions.Configuration => 41
	i64 u0x380134e03b1e160a, ; 115: System.Collections.Immutable.dll => 112
	i64 u0x385c17636bb6fe6e, ; 116: Xamarin.AndroidX.CustomView.dll => 83
	i64 u0x38869c811d74050e, ; 117: System.Net.NameResolution.dll => 136
	i64 u0x3889cbdca0f2c57c, ; 118: Xamarin.GooglePlayServices.Location.dll => 104
	i64 u0x393c226616977fdb, ; 119: lib_Xamarin.AndroidX.ViewPager.dll.so => 99
	i64 u0x395e37c3334cf82a, ; 120: lib-ca-Microsoft.Maui.Controls.resources.dll.so => 1
	i64 u0x39aa39fda111d9d3, ; 121: Newtonsoft.Json => 56
	i64 u0x3b9380b5cf400cdf, ; 122: Syncfusion.Maui.DataSource => 65
	i64 u0x3c7c495f58ac5ee9, ; 123: Xamarin.Kotlin.StdLib => 106
	i64 u0x3cd9d281d402eb9b, ; 124: Xamarin.AndroidX.Browser.dll => 77
	i64 u0x3d38b7e361e5e840, ; 125: Syncfusion.Maui.ListView.dll => 69
	i64 u0x3d46f0b995082740, ; 126: System.Xml.Linq => 164
	i64 u0x3d9c2a242b040a50, ; 127: lib_Xamarin.AndroidX.Core.dll.so => 81
	i64 u0x3e6c4bb2bd053e25, ; 128: Syncfusion.Maui.ListView => 69
	i64 u0x3fbc4f8f3a5cf3d4, ; 129: Syncfusion.Maui.GridCommon => 67
	i64 u0x407a10bb4bf95829, ; 130: lib_Xamarin.AndroidX.Navigation.Common.dll.so => 91
	i64 u0x41cab042be111c34, ; 131: lib_Xamarin.AndroidX.AppCompat.AppCompatResources.dll.so => 76
	i64 u0x43375950ec7c1b6a, ; 132: netstandard.dll => 167
	i64 u0x434c4e1d9284cdae, ; 133: Mono.Android.dll => 172
	i64 u0x43950f84de7cc79a, ; 134: pl/Microsoft.Maui.Controls.resources.dll => 20
	i64 u0x448bd33429269b19, ; 135: Microsoft.CSharp => 110
	i64 u0x4499fa3c8e494654, ; 136: lib_System.Runtime.Serialization.Primitives.dll.so => 154
	i64 u0x4515080865a951a5, ; 137: Xamarin.Kotlin.StdLib.dll => 106
	i64 u0x45c40276a42e283e, ; 138: System.Diagnostics.TraceSource => 123
	i64 u0x46a4213bc97fe5ae, ; 139: lib-ru-Microsoft.Maui.Controls.resources.dll.so => 24
	i64 u0x47358bd471172e1d, ; 140: lib_System.Xml.Linq.dll.so => 164
	i64 u0x478978bf61187a69, ; 141: Syncfusion.Maui.Charts => 63
	i64 u0x47daf4e1afbada10, ; 142: pt/Microsoft.Maui.Controls.resources => 22
	i64 u0x485c7d703a50c2a6, ; 143: lib_RGPopup.Maui.dll.so => 60
	i64 u0x49e952f19a4e2022, ; 144: System.ObjectModel => 146
	i64 u0x49f9e6948a8131e4, ; 145: lib_Xamarin.AndroidX.VersionedParcelable.dll.so => 98
	i64 u0x4a5667b2462a664b, ; 146: lib_Xamarin.AndroidX.Navigation.UI.dll.so => 94
	i64 u0x4a78a24dc5b649fc, ; 147: Syncfusion.Maui.Core.dll => 64
	i64 u0x4b7b6532ded934b7, ; 148: System.Text.Json => 158
	i64 u0x4c7755cf07ad2d5f, ; 149: System.Net.Http.Json.dll => 134
	i64 u0x4cc5f15266470798, ; 150: lib_Xamarin.AndroidX.Loader.dll.so => 90
	i64 u0x4cf6f67dc77aacd2, ; 151: System.Net.NetworkInformation.dll => 137
	i64 u0x4d29370cf4e8a157, ; 152: lib-en-US-Syncfusion.Maui.Inputs.resources.dll.so => 35
	i64 u0x4d3183dd245425d4, ; 153: System.Net.WebSockets.Client.dll => 143
	i64 u0x4d479f968a05e504, ; 154: System.Linq.Expressions.dll => 130
	i64 u0x4d55a010ffc4faff, ; 155: System.Private.Xml => 149
	i64 u0x4d91e5c949c8f5e5, ; 156: InputKit.Maui.dll => 39
	i64 u0x4d95fccc1f67c7ca, ; 157: System.Runtime.Loader.dll => 151
	i64 u0x4da4a8f0f6a70fdc, ; 158: Microsoft.Maui.Controls.Compatibility.dll => 50
	i64 u0x4dcf44c3c9b076a2, ; 159: it/Microsoft.Maui.Controls.resources.dll => 14
	i64 u0x4dd9247f1d2c3235, ; 160: Xamarin.AndroidX.Loader.dll => 90
	i64 u0x4e32f00cb0937401, ; 161: Mono.Android.Runtime => 171
	i64 u0x4ebd0c4b82c5eefc, ; 162: lib_System.Threading.Channels.dll.so => 160
	i64 u0x4f21ee6ef9eb527e, ; 163: ca/Microsoft.Maui.Controls.resources => 1
	i64 u0x5037f0be3c28c7a3, ; 164: lib_Microsoft.Maui.Controls.dll.so => 51
	i64 u0x5131bbe80989093f, ; 165: Xamarin.AndroidX.Lifecycle.ViewModel.Android.dll => 88
	i64 u0x51bb8a2afe774e32, ; 166: System.Drawing => 125
	i64 u0x526ce79eb8e90527, ; 167: lib_System.Net.Primitives.dll.so => 138
	i64 u0x52829f00b4467c38, ; 168: lib_System.Data.Common.dll.so => 120
	i64 u0x529ffe06f39ab8db, ; 169: Xamarin.AndroidX.Core => 81
	i64 u0x52ff996554dbf352, ; 170: Microsoft.Maui.Graphics => 55
	i64 u0x532f8a450e9a7c54, ; 171: lib_Syncfusion.Maui.ProgressBar.dll.so => 71
	i64 u0x535f7e40e8fef8af, ; 172: lib-sk-Microsoft.Maui.Controls.resources.dll.so => 25
	i64 u0x53a96d5c86c9e194, ; 173: System.Net.NetworkInformation => 137
	i64 u0x53c3014b9437e684, ; 174: lib-zh-HK-Microsoft.Maui.Controls.resources.dll.so => 31
	i64 u0x54795225dd1587af, ; 175: lib_System.Runtime.dll.so => 155
	i64 u0x556e8b63b660ab8b, ; 176: Xamarin.AndroidX.Lifecycle.Common.Jvm.dll => 86
	i64 u0x5588627c9a108ec9, ; 177: System.Collections.Specialized => 114
	i64 u0x571c5cfbec5ae8e2, ; 178: System.Private.Uri => 147
	i64 u0x579a06fed6eec900, ; 179: System.Private.CoreLib.dll => 168
	i64 u0x57c542c14049b66d, ; 180: System.Diagnostics.DiagnosticSource => 121
	i64 u0x58601b2dda4a27b9, ; 181: lib-ja-Microsoft.Maui.Controls.resources.dll.so => 15
	i64 u0x58688d9af496b168, ; 182: Microsoft.Extensions.DependencyInjection.dll => 43
	i64 u0x595a356d23e8da9a, ; 183: lib_Microsoft.CSharp.dll.so => 110
	i64 u0x5a89a886ae30258d, ; 184: lib_Xamarin.AndroidX.CoordinatorLayout.dll.so => 80
	i64 u0x5a8f6699f4a1caa9, ; 185: lib_System.Threading.dll.so => 162
	i64 u0x5ae9cd33b15841bf, ; 186: System.ComponentModel => 118
	i64 u0x5b5f0e240a06a2a2, ; 187: da/Microsoft.Maui.Controls.resources.dll => 3
	i64 u0x5b755276902c8414, ; 188: Xamarin.GooglePlayServices.Base => 102
	i64 u0x5c393624b8176517, ; 189: lib_Microsoft.Extensions.Logging.dll.so => 45
	i64 u0x5d0a4a29b02d9d3c, ; 190: System.Net.WebHeaderCollection.dll => 142
	i64 u0x5db0cbbd1028510e, ; 191: lib_System.Runtime.InteropServices.dll.so => 150
	i64 u0x5db30905d3e5013b, ; 192: Xamarin.AndroidX.Collection.Jvm.dll => 79
	i64 u0x5e467bc8f09ad026, ; 193: System.Collections.Specialized.dll => 114
	i64 u0x5ea92fdb19ec8c4c, ; 194: System.Text.Encodings.Web.dll => 157
	i64 u0x5eb8046dd40e9ac3, ; 195: System.ComponentModel.Primitives => 116
	i64 u0x5f36ccf5c6a57e24, ; 196: System.Xml.ReaderWriter.dll => 165
	i64 u0x5f4294b9b63cb842, ; 197: System.Data.Common => 120
	i64 u0x5f9a2d823f664957, ; 198: lib-el-Microsoft.Maui.Controls.resources.dll.so => 5
	i64 u0x609f4b7b63d802d4, ; 199: lib_Microsoft.Extensions.DependencyInjection.dll.so => 43
	i64 u0x60cd4e33d7e60134, ; 200: Xamarin.KotlinX.Coroutines.Core.Jvm => 107
	i64 u0x60f62d786afcf130, ; 201: System.Memory => 133
	i64 u0x61be8d1299194243, ; 202: Microsoft.Maui.Controls.Xaml => 52
	i64 u0x61d2cba29557038f, ; 203: de/Microsoft.Maui.Controls.resources => 4
	i64 u0x61d88f399afb2f45, ; 204: lib_System.Runtime.Loader.dll.so => 151
	i64 u0x622eef6f9e59068d, ; 205: System.Private.CoreLib => 168
	i64 u0x63f1f6883c1e23c2, ; 206: lib_System.Collections.Immutable.dll.so => 112
	i64 u0x6400f68068c1e9f1, ; 207: Xamarin.Google.Android.Material.dll => 101
	i64 u0x658f524e4aba7dad, ; 208: CommunityToolkit.Maui.dll => 37
	i64 u0x65ecac39144dd3cc, ; 209: Microsoft.Maui.Controls.dll => 51
	i64 u0x65ece51227bfa724, ; 210: lib_System.Runtime.Numerics.dll.so => 152
	i64 u0x6647e4302ad32b11, ; 211: lib_Syncfusion.Maui.DataSource.dll.so => 65
	i64 u0x6692e924eade1b29, ; 212: lib_System.Console.dll.so => 119
	i64 u0x66a4e5c6a3fb0bae, ; 213: lib_Xamarin.AndroidX.Lifecycle.ViewModel.Android.dll.so => 88
	i64 u0x66d13304ce1a3efa, ; 214: Xamarin.AndroidX.CursorAdapter => 82
	i64 u0x68558ec653afa616, ; 215: lib-da-Microsoft.Maui.Controls.resources.dll.so => 3
	i64 u0x6872ec7a2e36b1ac, ; 216: System.Drawing.Primitives.dll => 124
	i64 u0x68fbbbe2eb455198, ; 217: System.Formats.Asn1 => 126
	i64 u0x69063fc0ba8e6bdd, ; 218: he/Microsoft.Maui.Controls.resources.dll => 9
	i64 u0x69166eb623aa58f2, ; 219: Syncfusion.Maui.DataSource.dll => 65
	i64 u0x6a4d7577b2317255, ; 220: System.Runtime.InteropServices.dll => 150
	i64 u0x6ace3b74b15ee4a4, ; 221: nb/Microsoft.Maui.Controls.resources => 18
	i64 u0x6d12bfaa99c72b1f, ; 222: lib_Microsoft.Maui.Graphics.dll.so => 55
	i64 u0x6d79993361e10ef2, ; 223: Microsoft.Extensions.Primitives => 49
	i64 u0x6d86d56b84c8eb71, ; 224: lib_Xamarin.AndroidX.CursorAdapter.dll.so => 82
	i64 u0x6d9bea6b3e895cf7, ; 225: Microsoft.Extensions.Primitives.dll => 49
	i64 u0x6e25a02c3833319a, ; 226: lib_Xamarin.AndroidX.Navigation.Fragment.dll.so => 92
	i64 u0x6fd2265da78b93a4, ; 227: lib_Microsoft.Maui.dll.so => 53
	i64 u0x6fdfc7de82c33008, ; 228: cs/Microsoft.Maui.Controls.resources => 2
	i64 u0x7044c72535833f90, ; 229: en-US/Syncfusion.Maui.Buttons.resources.dll => 34
	i64 u0x70e99f48c05cb921, ; 230: tr/Microsoft.Maui.Controls.resources.dll => 28
	i64 u0x70fd3deda22442d2, ; 231: lib-nb-Microsoft.Maui.Controls.resources.dll.so => 18
	i64 u0x71a495ea3761dde8, ; 232: lib-it-Microsoft.Maui.Controls.resources.dll.so => 14
	i64 u0x71ad672adbe48f35, ; 233: System.ComponentModel.Primitives.dll => 116
	i64 u0x72b1fb4109e08d7b, ; 234: lib-hr-Microsoft.Maui.Controls.resources.dll.so => 11
	i64 u0x73e4ce94e2eb6ffc, ; 235: lib_System.Memory.dll.so => 133
	i64 u0x74fcb5b9d3ee6884, ; 236: Plugin.LocalNotification => 58
	i64 u0x755a91767330b3d4, ; 237: lib_Microsoft.Extensions.Configuration.dll.so => 41
	i64 u0x76012e7334db86e5, ; 238: lib_Xamarin.AndroidX.SavedState.dll.so => 96
	i64 u0x76ca07b878f44da0, ; 239: System.Runtime.Numerics.dll => 152
	i64 u0x780bc73597a503a9, ; 240: lib-ms-Microsoft.Maui.Controls.resources.dll.so => 17
	i64 u0x783606d1e53e7a1a, ; 241: th/Microsoft.Maui.Controls.resources.dll => 27
	i64 u0x78a45e51311409b6, ; 242: Xamarin.AndroidX.Fragment.dll => 85
	i64 u0x7a090e7cbb6c0ed1, ; 243: Xamarin.GooglePlayServices.Location => 104
	i64 u0x7adb8da2ac89b647, ; 244: fi/Microsoft.Maui.Controls.resources.dll => 7
	i64 u0x7bef86a4335c4870, ; 245: System.ComponentModel.TypeConverter => 117
	i64 u0x7c0820144cd34d6a, ; 246: sk/Microsoft.Maui.Controls.resources.dll => 25
	i64 u0x7c2a0bd1e0f988fc, ; 247: lib-de-Microsoft.Maui.Controls.resources.dll.so => 4
	i64 u0x7cb95ad2a929d044, ; 248: Xamarin.GooglePlayServices.Basement => 103
	i64 u0x7cc637f941f716d0, ; 249: CommunityToolkit.Maui.Core => 38
	i64 u0x7d649b75d580bb42, ; 250: ms/Microsoft.Maui.Controls.resources.dll => 17
	i64 u0x7d8ee2bdc8e3aad1, ; 251: System.Numerics.Vectors => 145
	i64 u0x7df5df8db8eaa6ac, ; 252: Microsoft.Extensions.Logging.Debug => 47
	i64 u0x7dfc3d6d9d8d7b70, ; 253: System.Collections => 115
	i64 u0x7e946809d6008ef2, ; 254: lib_System.ObjectModel.dll.so => 146
	i64 u0x7eb4f0dc47488736, ; 255: lib_Xamarin.GooglePlayServices.Tasks.dll.so => 105
	i64 u0x7ecc13347c8fd849, ; 256: lib_System.ComponentModel.dll.so => 118
	i64 u0x7ef63e15ff9d3605, ; 257: PINView.Maui.dll => 57
	i64 u0x7f00ddd9b9ca5a13, ; 258: Xamarin.AndroidX.ViewPager.dll => 99
	i64 u0x7f9351cd44b1273f, ; 259: Microsoft.Extensions.Configuration.Abstractions => 42
	i64 u0x7fbd557c99b3ce6f, ; 260: lib_Xamarin.AndroidX.Lifecycle.LiveData.Core.dll.so => 87
	i64 u0x809b1d586a1c34b9, ; 261: Plugin.Maui.Audio => 59
	i64 u0x812c069d5cdecc17, ; 262: System.dll => 166
	i64 u0x81ab745f6c0f5ce6, ; 263: zh-Hant/Microsoft.Maui.Controls.resources => 33
	i64 u0x8277f2be6b5ce05f, ; 264: Xamarin.AndroidX.AppCompat => 75
	i64 u0x828f06563b30bc50, ; 265: lib_Xamarin.AndroidX.CardView.dll.so => 78
	i64 u0x82df8f5532a10c59, ; 266: lib_System.Drawing.dll.so => 125
	i64 u0x82f6403342e12049, ; 267: uk/Microsoft.Maui.Controls.resources => 29
	i64 u0x83c14ba66c8e2b8c, ; 268: zh-Hans/Microsoft.Maui.Controls.resources => 32
	i64 u0x86a909228dc7657b, ; 269: lib-zh-Hant-Microsoft.Maui.Controls.resources.dll.so => 33
	i64 u0x86b3e00c36b84509, ; 270: Microsoft.Extensions.Configuration.dll => 41
	i64 u0x87a3c575cf2318ce, ; 271: Syncfusion.Maui.Sliders.dll => 72
	i64 u0x87c69b87d9283884, ; 272: lib_System.Threading.Thread.dll.so => 161
	i64 u0x87f6569b25707834, ; 273: System.IO.Compression.Brotli.dll => 127
	i64 u0x8842b3a5d2d3fb36, ; 274: Microsoft.Maui.Essentials => 54
	i64 u0x88bda98e0cffb7a9, ; 275: lib_Xamarin.KotlinX.Coroutines.Core.Jvm.dll.so => 107
	i64 u0x8930322c7bd8f768, ; 276: netstandard => 167
	i64 u0x897a606c9e39c75f, ; 277: lib_System.ComponentModel.Primitives.dll.so => 116
	i64 u0x8ac8d025b93e29e9, ; 278: Syncfusion.Licensing => 61
	i64 u0x8ad229ea26432ee2, ; 279: Xamarin.AndroidX.Loader => 90
	i64 u0x8b4ff5d0fdd5faa1, ; 280: lib_System.Diagnostics.DiagnosticSource.dll.so => 121
	i64 u0x8b8d01333a96d0b5, ; 281: System.Diagnostics.Process.dll => 122
	i64 u0x8b9ceca7acae3451, ; 282: lib-he-Microsoft.Maui.Controls.resources.dll.so => 9
	i64 u0x8d0f420977c2c1c7, ; 283: Xamarin.AndroidX.CursorAdapter.dll => 82
	i64 u0x8d7b8ab4b3310ead, ; 284: System.Threading => 162
	i64 u0x8da188285aadfe8e, ; 285: System.Collections.Concurrent => 111
	i64 u0x8ec6e06a61c1baeb, ; 286: lib_Newtonsoft.Json.dll.so => 56
	i64 u0x8ed807bfe9858dfc, ; 287: Xamarin.AndroidX.Navigation.Common => 91
	i64 u0x8ee08b8194a30f48, ; 288: lib-hi-Microsoft.Maui.Controls.resources.dll.so => 10
	i64 u0x8ef7601039857a44, ; 289: lib-ro-Microsoft.Maui.Controls.resources.dll.so => 23
	i64 u0x8efbc0801a122264, ; 290: Xamarin.GooglePlayServices.Tasks.dll => 105
	i64 u0x8f32c6f611f6ffab, ; 291: pt/Microsoft.Maui.Controls.resources.dll => 22
	i64 u0x8f8829d21c8985a4, ; 292: lib-pt-BR-Microsoft.Maui.Controls.resources.dll.so => 21
	i64 u0x90263f8448b8f572, ; 293: lib_System.Diagnostics.TraceSource.dll.so => 123
	i64 u0x902d31dbd7d7d78e, ; 294: InputKit.Maui => 39
	i64 u0x903101b46fb73a04, ; 295: _Microsoft.Android.Resource.Designer => 36
	i64 u0x90393bd4865292f3, ; 296: lib_System.IO.Compression.dll.so => 128
	i64 u0x90634f86c5ebe2b5, ; 297: Xamarin.AndroidX.Lifecycle.ViewModel.Android => 88
	i64 u0x907b636704ad79ef, ; 298: lib_Microsoft.Maui.Controls.Xaml.dll.so => 52
	i64 u0x91418dc638b29e68, ; 299: lib_Xamarin.AndroidX.CustomView.dll.so => 83
	i64 u0x9157bd523cd7ed36, ; 300: lib_System.Text.Json.dll.so => 158
	i64 u0x91a74f07b30d37e2, ; 301: System.Linq.dll => 132
	i64 u0x91fa41a87223399f, ; 302: ca/Microsoft.Maui.Controls.resources.dll => 1
	i64 u0x93cfa73ab28d6e35, ; 303: ms/Microsoft.Maui.Controls.resources => 17
	i64 u0x944077d8ca3c6580, ; 304: System.IO.Compression.dll => 128
	i64 u0x967fc325e09bfa8c, ; 305: es/Microsoft.Maui.Controls.resources => 6
	i64 u0x9732d8dbddea3d9a, ; 306: id/Microsoft.Maui.Controls.resources => 13
	i64 u0x978be80e5210d31b, ; 307: Microsoft.Maui.Graphics.dll => 55
	i64 u0x979ab54025cc1c7f, ; 308: lib_Xamarin.GooglePlayServices.Base.dll.so => 102
	i64 u0x97b8c771ea3e4220, ; 309: System.ComponentModel.dll => 118
	i64 u0x97e144c9d3c6976e, ; 310: System.Collections.Concurrent.dll => 111
	i64 u0x991d510397f92d9d, ; 311: System.Linq.Expressions => 130
	i64 u0x993cc632e821c001, ; 312: Microsoft.Maui.Controls.Compatibility => 50
	i64 u0x99a00ca5270c6878, ; 313: Xamarin.AndroidX.Navigation.Runtime => 93
	i64 u0x99cdc6d1f2d3a72f, ; 314: ko/Microsoft.Maui.Controls.resources.dll => 16
	i64 u0x9b24965fcda80df2, ; 315: lib_PINView.Maui.dll.so => 57
	i64 u0x9b319e71c7ea0b90, ; 316: lib_Syncfusion.Maui.Buttons.dll.so => 62
	i64 u0x9cd79a4146324d25, ; 317: lib_Syncfusion.Maui.Inputs.dll.so => 68
	i64 u0x9d5dbcf5a48583fe, ; 318: lib_Xamarin.AndroidX.Activity.dll.so => 74
	i64 u0x9d74dee1a7725f34, ; 319: Microsoft.Extensions.Configuration.Abstractions.dll => 42
	i64 u0x9e4534b6adaf6e84, ; 320: nl/Microsoft.Maui.Controls.resources => 19
	i64 u0x9eaf1efdf6f7267e, ; 321: Xamarin.AndroidX.Navigation.Common.dll => 91
	i64 u0x9ef542cf1f78c506, ; 322: Xamarin.AndroidX.Lifecycle.LiveData.Core => 87
	i64 u0x9f331d415d8d49d2, ; 323: Syncfusion.Maui.Inputs => 68
	i64 u0x9f5c7301a67b9123, ; 324: lib_Syncfusion.Maui.Sliders.dll.so => 72
	i64 u0x9fc2184212c417ad, ; 325: Plugin.LocalNotification.dll => 58
	i64 u0x9fcf56f4f96952bc, ; 326: lib_Syncfusion.Maui.ListView.dll.so => 69
	i64 u0xa0d8259f4cc284ec, ; 327: lib_System.Security.Cryptography.dll.so => 156
	i64 u0xa1440773ee9d341e, ; 328: Xamarin.Google.Android.Material => 101
	i64 u0xa1b9d7c27f47219f, ; 329: Xamarin.AndroidX.Navigation.UI.dll => 94
	i64 u0xa2572680829d2c7c, ; 330: System.IO.Pipelines.dll => 129
	i64 u0xa46aa1eaa214539b, ; 331: ko/Microsoft.Maui.Controls.resources => 16
	i64 u0xa4edc8f2ceae241a, ; 332: System.Data.Common.dll => 120
	i64 u0xa5494f40f128ce6a, ; 333: System.Runtime.Serialization.Formatters.dll => 153
	i64 u0xa5e599d1e0524750, ; 334: System.Numerics.Vectors.dll => 145
	i64 u0xa5f1ba49b85dd355, ; 335: System.Security.Cryptography.dll => 156
	i64 u0xa602550d26ecaf11, ; 336: Syncfusion.Maui.ProgressBar => 71
	i64 u0xa67dbee13e1df9ca, ; 337: Xamarin.AndroidX.SavedState.dll => 96
	i64 u0xa68a420042bb9b1f, ; 338: Xamarin.AndroidX.DrawerLayout.dll => 84
	i64 u0xa78ce3745383236a, ; 339: Xamarin.AndroidX.Lifecycle.Common.Jvm => 86
	i64 u0xa7c31b56b4dc7b33, ; 340: hu/Microsoft.Maui.Controls.resources => 12
	i64 u0xa7eab29ed44b4e7a, ; 341: Mono.Android.Export => 170
	i64 u0xa843f6095f0d247d, ; 342: Xamarin.GooglePlayServices.Base.dll => 102
	i64 u0xa964304b5631e28a, ; 343: CommunityToolkit.Maui.Core.dll => 38
	i64 u0xaa2219c8e3449ff5, ; 344: Microsoft.Extensions.Logging.Abstractions => 46
	i64 u0xaa443ac34067eeef, ; 345: System.Private.Xml.dll => 149
	i64 u0xaa52de307ef5d1dd, ; 346: System.Net.Http => 135
	i64 u0xaaaf86367285a918, ; 347: Microsoft.Extensions.DependencyInjection.Abstractions.dll => 44
	i64 u0xaaf84bb3f052a265, ; 348: el/Microsoft.Maui.Controls.resources => 5
	i64 u0xab9c1b2687d86b0b, ; 349: lib_System.Linq.Expressions.dll.so => 130
	i64 u0xac2af3fa195a15ce, ; 350: System.Runtime.Numerics => 152
	i64 u0xac5376a2a538dc10, ; 351: Xamarin.AndroidX.Lifecycle.LiveData.Core.dll => 87
	i64 u0xacd46e002c3ccb97, ; 352: ro/Microsoft.Maui.Controls.resources => 23
	i64 u0xacf42eea7ef9cd12, ; 353: System.Threading.Channels => 160
	i64 u0xad89c07347f1bad6, ; 354: nl/Microsoft.Maui.Controls.resources.dll => 19
	i64 u0xadbb53caf78a79d2, ; 355: System.Web.HttpUtility => 163
	i64 u0xadc90ab061a9e6e4, ; 356: System.ComponentModel.TypeConverter.dll => 117
	i64 u0xade68809fa6b897c, ; 357: en-US/Syncfusion.Maui.Inputs.resources.dll => 35
	i64 u0xadf511667bef3595, ; 358: System.Net.Security => 140
	i64 u0xae282bcd03739de7, ; 359: Java.Interop => 169
	i64 u0xae53579c90db1107, ; 360: System.ObjectModel.dll => 146
	i64 u0xafe29f45095518e7, ; 361: lib_Xamarin.AndroidX.Lifecycle.ViewModelSavedState.dll.so => 89
	i64 u0xb04ea60a308db4ff, ; 362: lib_Syncfusion.Maui.Popup.dll.so => 70
	i64 u0xb05cc42cd94c6d9d, ; 363: lib-sv-Microsoft.Maui.Controls.resources.dll.so => 26
	i64 u0xb220631954820169, ; 364: System.Text.RegularExpressions => 159
	i64 u0xb2a3f67f3bf29fce, ; 365: da/Microsoft.Maui.Controls.resources => 3
	i64 u0xb306a467ede01e48, ; 366: Syncfusion.Maui.Popup.dll => 70
	i64 u0xb3091da7ade38b8a, ; 367: RGPopup.Maui.dll => 60
	i64 u0xb31cc4d55f1964f2, ; 368: Syncfusion.Maui.Buttons => 62
	i64 u0xb345c84ec35298e7, ; 369: Syncfusion.Maui.Popup => 70
	i64 u0xb3b9014870e7b69c, ; 370: Microsoft.AspNet.SignalR.Client => 40
	i64 u0xb3f0a0fcda8d3ebc, ; 371: Xamarin.AndroidX.CardView => 78
	i64 u0xb3f832258cb83db4, ; 372: Syncfusion.Licensing.dll => 61
	i64 u0xb46be1aa6d4fff93, ; 373: hi/Microsoft.Maui.Controls.resources => 10
	i64 u0xb477491be13109d8, ; 374: ar/Microsoft.Maui.Controls.resources => 0
	i64 u0xb4bd7015ecee9d86, ; 375: System.IO.Pipelines => 129
	i64 u0xb5c7fcdafbc67ee4, ; 376: Microsoft.Extensions.Logging.Abstractions.dll => 46
	i64 u0xb6df9821aa4e6191, ; 377: SFKBle_Admin.dll => 109
	i64 u0xb7212c4683a94afe, ; 378: System.Drawing.Primitives => 124
	i64 u0xb7b7753d1f319409, ; 379: sv/Microsoft.Maui.Controls.resources => 26
	i64 u0xb81a2c6e0aee50fe, ; 380: lib_System.Private.CoreLib.dll.so => 168
	i64 u0xb9185c33a1643eed, ; 381: Microsoft.CSharp.dll => 110
	i64 u0xb9f64d3b230def68, ; 382: lib-pt-Microsoft.Maui.Controls.resources.dll.so => 22
	i64 u0xb9fc3c8a556e3691, ; 383: ja/Microsoft.Maui.Controls.resources => 15
	i64 u0xba48785529705af9, ; 384: System.Collections.dll => 115
	i64 u0xbb65706fde942ce3, ; 385: System.Net.Sockets => 141
	i64 u0xbbd180354b67271a, ; 386: System.Runtime.Serialization.Formatters => 153
	i64 u0xbd0e2c0d55246576, ; 387: System.Net.Http.dll => 135
	i64 u0xbd437a2cdb333d0d, ; 388: Xamarin.AndroidX.ViewPager2 => 100
	i64 u0xbd5d0b88d3d647a5, ; 389: lib_Xamarin.AndroidX.Browser.dll.so => 77
	i64 u0xbe381ebc74cde412, ; 390: Syncfusion.Maui.TabView => 73
	i64 u0xbee38d4a88835966, ; 391: Xamarin.AndroidX.AppCompat.AppCompatResources => 76
	i64 u0xbfc1e1fb3095f2b3, ; 392: lib_System.Net.Http.Json.dll.so => 134
	i64 u0xc040a4ab55817f58, ; 393: ar/Microsoft.Maui.Controls.resources.dll => 0
	i64 u0xc0d928351ab5ca77, ; 394: System.Console.dll => 119
	i64 u0xc12b8b3afa48329c, ; 395: lib_System.Linq.dll.so => 132
	i64 u0xc1ff9ae3cdb6e1e6, ; 396: Xamarin.AndroidX.Activity.dll => 74
	i64 u0xc28c50f32f81cc73, ; 397: ja/Microsoft.Maui.Controls.resources.dll => 15
	i64 u0xc2902f6cf5452577, ; 398: lib_Mono.Android.Export.dll.so => 170
	i64 u0xc2bcfec99f69365e, ; 399: Xamarin.AndroidX.ViewPager2.dll => 100
	i64 u0xc421b61fd853169d, ; 400: lib_System.Net.WebSockets.Client.dll.so => 143
	i64 u0xc4d3858ed4d08512, ; 401: Xamarin.AndroidX.Lifecycle.ViewModelSavedState.dll => 89
	i64 u0xc50fded0ded1418c, ; 402: lib_System.ComponentModel.TypeConverter.dll.so => 117
	i64 u0xc519125d6bc8fb11, ; 403: lib_System.Net.Requests.dll.so => 139
	i64 u0xc5293b19e4dc230e, ; 404: Xamarin.AndroidX.Navigation.Fragment => 92
	i64 u0xc5325b2fcb37446f, ; 405: lib_System.Private.Xml.dll.so => 149
	i64 u0xc5a0f4b95a699af7, ; 406: lib_System.Private.Uri.dll.so => 147
	i64 u0xc7ce851898a4548e, ; 407: lib_System.Web.HttpUtility.dll.so => 163
	i64 u0xc858a28d9ee5a6c5, ; 408: lib_System.Collections.Specialized.dll.so => 114
	i64 u0xc9e54b32fc19baf3, ; 409: lib_CommunityToolkit.Maui.dll.so => 37
	i64 u0xca3a723e7342c5b6, ; 410: lib-tr-Microsoft.Maui.Controls.resources.dll.so => 28
	i64 u0xcab3493c70141c2d, ; 411: pl/Microsoft.Maui.Controls.resources => 20
	i64 u0xcac6bda44c2b4e1e, ; 412: lib_Syncfusion.Maui.Charts.dll.so => 63
	i64 u0xcacfddc9f7c6de76, ; 413: ro/Microsoft.Maui.Controls.resources.dll => 23
	i64 u0xcbd4fdd9cef4a294, ; 414: lib__Microsoft.Android.Resource.Designer.dll.so => 36
	i64 u0xcc2876b32ef2794c, ; 415: lib_System.Text.RegularExpressions.dll.so => 159
	i64 u0xcc5c3bb714c4561e, ; 416: Xamarin.KotlinX.Coroutines.Core.Jvm.dll => 107
	i64 u0xcc76886e09b88260, ; 417: Xamarin.KotlinX.Serialization.Core.Jvm.dll => 108
	i64 u0xccf25c4b634ccd3a, ; 418: zh-Hans/Microsoft.Maui.Controls.resources.dll => 32
	i64 u0xcd10a42808629144, ; 419: System.Net.Requests => 139
	i64 u0xcdd0c48b6937b21c, ; 420: Xamarin.AndroidX.SwipeRefreshLayout => 97
	i64 u0xcf23d8093f3ceadf, ; 421: System.Diagnostics.DiagnosticSource.dll => 121
	i64 u0xcf8fc898f98b0d34, ; 422: System.Private.Xml.Linq => 148
	i64 u0xd1194e1d8a8de83c, ; 423: lib_Xamarin.AndroidX.Lifecycle.Common.Jvm.dll.so => 86
	i64 u0xd333d0af9e423810, ; 424: System.Runtime.InteropServices => 150
	i64 u0xd3426d966bb704f5, ; 425: Xamarin.AndroidX.AppCompat.AppCompatResources.dll => 76
	i64 u0xd3651b6fc3125825, ; 426: System.Private.Uri.dll => 147
	i64 u0xd373685349b1fe8b, ; 427: Microsoft.Extensions.Logging.dll => 45
	i64 u0xd3e4c8d6a2d5d470, ; 428: it/Microsoft.Maui.Controls.resources => 14
	i64 u0xd4645626dffec99d, ; 429: lib_Microsoft.Extensions.DependencyInjection.Abstractions.dll.so => 44
	i64 u0xd5507e11a2b2839f, ; 430: Xamarin.AndroidX.Lifecycle.ViewModelSavedState => 89
	i64 u0xd557d5227269bee0, ; 431: lib_Syncfusion.Maui.GridCommon.dll.so => 67
	i64 u0xd60815f26a12e140, ; 432: Microsoft.Extensions.Logging.Debug.dll => 47
	i64 u0xd6694f8359737e4e, ; 433: Xamarin.AndroidX.SavedState => 96
	i64 u0xd6d21782156bc35b, ; 434: Xamarin.AndroidX.SwipeRefreshLayout.dll => 97
	i64 u0xd72329819cbbbc44, ; 435: lib_Microsoft.Extensions.Configuration.Abstractions.dll.so => 42
	i64 u0xd7b3764ada9d341d, ; 436: lib_Microsoft.Extensions.Logging.Abstractions.dll.so => 46
	i64 u0xd7f0088bc5ad71f2, ; 437: Xamarin.AndroidX.VersionedParcelable => 98
	i64 u0xd88e1a735502b528, ; 438: en-US/Syncfusion.Maui.Inputs.resources => 35
	i64 u0xd9506f12086c6133, ; 439: lib_Syncfusion.Maui.TabView.dll.so => 73
	i64 u0xda1dfa4c534a9251, ; 440: Microsoft.Extensions.DependencyInjection => 43
	i64 u0xdad05a11827959a3, ; 441: System.Collections.NonGeneric.dll => 113
	i64 u0xdb5383ab5865c007, ; 442: lib-vi-Microsoft.Maui.Controls.resources.dll.so => 30
	i64 u0xdbeda89f832aa805, ; 443: vi/Microsoft.Maui.Controls.resources.dll => 30
	i64 u0xdbf9607a441b4505, ; 444: System.Linq => 132
	i64 u0xdca8be7403f92d4f, ; 445: lib_System.Linq.Queryable.dll.so => 131
	i64 u0xdce2c53525640bf3, ; 446: Microsoft.Extensions.Logging => 45
	i64 u0xdd2b722d78ef5f43, ; 447: System.Runtime.dll => 155
	i64 u0xdd67031857c72f96, ; 448: lib_System.Text.Encodings.Web.dll.so => 157
	i64 u0xdde30e6b77aa6f6c, ; 449: lib-zh-Hans-Microsoft.Maui.Controls.resources.dll.so => 32
	i64 u0xde8769ebda7d8647, ; 450: hr/Microsoft.Maui.Controls.resources.dll => 11
	i64 u0xded7435ceb96ed5c, ; 451: SFKBle_Admin => 109
	i64 u0xe0142572c095a480, ; 452: Xamarin.AndroidX.AppCompat.dll => 75
	i64 u0xe02f89350ec78051, ; 453: Xamarin.AndroidX.CoordinatorLayout.dll => 80
	i64 u0xe192a588d4410686, ; 454: lib_System.IO.Pipelines.dll.so => 129
	i64 u0xe1a08bd3fa539e0d, ; 455: System.Runtime.Loader => 151
	i64 u0xe1b52f9f816c70ef, ; 456: System.Private.Xml.Linq.dll => 148
	i64 u0xe1ecfdb7fff86067, ; 457: System.Net.Security.dll => 140
	i64 u0xe2420585aeceb728, ; 458: System.Net.Requests.dll => 139
	i64 u0xe29b73bc11392966, ; 459: lib-id-Microsoft.Maui.Controls.resources.dll.so => 13
	i64 u0xe332bacb3eb4a806, ; 460: Mono.Android.Export.dll => 170
	i64 u0xe3811d68d4fe8463, ; 461: pt-BR/Microsoft.Maui.Controls.resources.dll => 21
	i64 u0xe4507486c308efd4, ; 462: lib_Xamarin.GooglePlayServices.Location.dll.so => 104
	i64 u0xe494f7ced4ecd10a, ; 463: hu/Microsoft.Maui.Controls.resources.dll => 12
	i64 u0xe4a9b1e40d1e8917, ; 464: lib-fi-Microsoft.Maui.Controls.resources.dll.so => 7
	i64 u0xe4f74a0b5bf9703f, ; 465: System.Runtime.Serialization.Primitives => 154
	i64 u0xe5434e8a119ceb69, ; 466: lib_Mono.Android.dll.so => 172
	i64 u0xe7e0f6010ea64be5, ; 467: en-US/Syncfusion.Maui.Buttons.resources => 34
	i64 u0xe7e7d98eda944101, ; 468: Syncfusion.Maui.Sliders => 72
	i64 u0xe8557f8edfc6f97b, ; 469: lib_SFKBle_Admin.dll.so => 109
	i64 u0xe86af59e77cc4cf1, ; 470: PINView.Maui => 57
	i64 u0xe89a2a9ef110899b, ; 471: System.Drawing.dll => 125
	i64 u0xed19c616b3fcb7eb, ; 472: Xamarin.AndroidX.VersionedParcelable.dll => 98
	i64 u0xedc4817167106c23, ; 473: System.Net.Sockets.dll => 141
	i64 u0xedc632067fb20ff3, ; 474: System.Memory.dll => 133
	i64 u0xedc8e4ca71a02a8b, ; 475: Xamarin.AndroidX.Navigation.Runtime.dll => 93
	i64 u0xeeb7ebb80150501b, ; 476: lib_Xamarin.AndroidX.Collection.Jvm.dll.so => 79
	i64 u0xef72742e1bcca27a, ; 477: Microsoft.Maui.Essentials.dll => 54
	i64 u0xefec0b7fdc57ec42, ; 478: Xamarin.AndroidX.Activity => 74
	i64 u0xf00c29406ea45e19, ; 479: es/Microsoft.Maui.Controls.resources.dll => 6
	i64 u0xf09e47b6ae914f6e, ; 480: System.Net.NameResolution => 136
	i64 u0xf0de2537ee19c6ca, ; 481: lib_System.Net.WebHeaderCollection.dll.so => 142
	i64 u0xf11b621fc87b983f, ; 482: Microsoft.Maui.Controls.Xaml.dll => 52
	i64 u0xf1c4b4005493d871, ; 483: System.Formats.Asn1.dll => 126
	i64 u0xf238bd79489d3a96, ; 484: lib-nl-Microsoft.Maui.Controls.resources.dll.so => 19
	i64 u0xf37221fda4ef8830, ; 485: lib_Xamarin.Google.Android.Material.dll.so => 101
	i64 u0xf3ddfe05336abf29, ; 486: System => 166
	i64 u0xf3e7deecd8959184, ; 487: Syncfusion.Maui.Gauges => 66
	i64 u0xf4103170a1de5bd0, ; 488: System.Linq.Queryable.dll => 131
	i64 u0xf437b94630896f3f, ; 489: lib_Plugin.Maui.Audio.dll.so => 59
	i64 u0xf4c1dd70a5496a17, ; 490: System.IO.Compression => 128
	i64 u0xf5fc7602fe27b333, ; 491: System.Net.WebHeaderCollection => 142
	i64 u0xf6077741019d7428, ; 492: Xamarin.AndroidX.CoordinatorLayout => 80
	i64 u0xf77b20923f07c667, ; 493: de/Microsoft.Maui.Controls.resources.dll => 4
	i64 u0xf7e2cac4c45067b3, ; 494: lib_System.Numerics.Vectors.dll.so => 145
	i64 u0xf7e74930e0e3d214, ; 495: zh-HK/Microsoft.Maui.Controls.resources.dll => 31
	i64 u0xf7fa0bf77fe677cc, ; 496: Newtonsoft.Json.dll => 56
	i64 u0xf84773b5c81e3cef, ; 497: lib-uk-Microsoft.Maui.Controls.resources.dll.so => 29
	i64 u0xf8aac5ea82de1348, ; 498: System.Linq.Queryable => 131
	i64 u0xf8e045dc345b2ea3, ; 499: lib_Xamarin.AndroidX.RecyclerView.dll.so => 95
	i64 u0xf915dc29808193a1, ; 500: System.Web.HttpUtility.dll => 163
	i64 u0xf91db4f1ad5d43b1, ; 501: Plugin.Maui.Audio.dll => 59
	i64 u0xf96c777a2a0686f4, ; 502: hi/Microsoft.Maui.Controls.resources.dll => 10
	i64 u0xf9eec5bb3a6aedc6, ; 503: Microsoft.Extensions.Options => 48
	i64 u0xfa3f278f288b0e84, ; 504: lib_System.Net.Security.dll.so => 140
	i64 u0xfa5ed7226d978949, ; 505: lib-ar-Microsoft.Maui.Controls.resources.dll.so => 0
	i64 u0xfa645d91e9fc4cba, ; 506: System.Threading.Thread => 161
	i64 u0xfb2ece4afed7c7c3, ; 507: Syncfusion.Maui.Inputs.dll => 68
	i64 u0xfb3cb754cb2d9fc0, ; 508: lib_Plugin.LocalNotification.dll.so => 58
	i64 u0xfb80f787c8458bd5, ; 509: lib-en-US-Syncfusion.Maui.Buttons.resources.dll.so => 34
	i64 u0xfbf0a31c9fc34bc4, ; 510: lib_System.Net.Http.dll.so => 135
	i64 u0xfc6b7527cc280b3f, ; 511: lib_System.Runtime.Serialization.Formatters.dll.so => 153
	i64 u0xfc719aec26adf9d9, ; 512: Xamarin.AndroidX.Navigation.Fragment.dll => 92
	i64 u0xfd22f00870e40ae0, ; 513: lib_Xamarin.AndroidX.DrawerLayout.dll.so => 84
	i64 u0xfd583f7657b6a1cb, ; 514: Xamarin.AndroidX.Fragment => 85
	i64 u0xfda36abccf05cf5c, ; 515: System.Net.WebSockets.Client => 143
	i64 u0xfdbe4710aa9beeff, ; 516: CommunityToolkit.Maui => 37
	i64 u0xfeae9952cf03b8cb, ; 517: tr/Microsoft.Maui.Controls.resources => 28
	i64 u0xff9b54613e0d2cc8 ; 518: System.Net.Http.Json => 134
], align 8

@assembly_image_cache_indices = dso_local local_unnamed_addr constant [519 x i32] [
	i32 97, i32 93, i32 38, i32 171, i32 75, i32 60, i32 24, i32 2,
	i32 30, i32 138, i32 105, i32 95, i32 115, i32 53, i32 31, i32 164,
	i32 79, i32 24, i32 113, i32 47, i32 84, i32 48, i32 113, i32 103,
	i32 156, i32 73, i32 160, i32 25, i32 108, i32 99, i32 21, i32 172,
	i32 54, i32 136, i32 83, i32 64, i32 127, i32 63, i32 144, i32 95,
	i32 66, i32 77, i32 8, i32 169, i32 9, i32 44, i32 144, i32 64,
	i32 167, i32 12, i32 157, i32 108, i32 67, i32 18, i32 66, i32 111,
	i32 166, i32 27, i32 171, i32 50, i32 94, i32 16, i32 48, i32 127,
	i32 122, i32 155, i32 62, i32 27, i32 103, i32 161, i32 119, i32 81,
	i32 154, i32 8, i32 106, i32 49, i32 13, i32 11, i32 144, i32 169,
	i32 138, i32 40, i32 29, i32 40, i32 137, i32 123, i32 7, i32 159,
	i32 126, i32 33, i32 20, i32 148, i32 61, i32 162, i32 26, i32 158,
	i32 5, i32 122, i32 165, i32 85, i32 36, i32 78, i32 124, i32 8,
	i32 165, i32 112, i32 6, i32 71, i32 141, i32 39, i32 53, i32 2,
	i32 51, i32 100, i32 41, i32 112, i32 83, i32 136, i32 104, i32 99,
	i32 1, i32 56, i32 65, i32 106, i32 77, i32 69, i32 164, i32 81,
	i32 69, i32 67, i32 91, i32 76, i32 167, i32 172, i32 20, i32 110,
	i32 154, i32 106, i32 123, i32 24, i32 164, i32 63, i32 22, i32 60,
	i32 146, i32 98, i32 94, i32 64, i32 158, i32 134, i32 90, i32 137,
	i32 35, i32 143, i32 130, i32 149, i32 39, i32 151, i32 50, i32 14,
	i32 90, i32 171, i32 160, i32 1, i32 51, i32 88, i32 125, i32 138,
	i32 120, i32 81, i32 55, i32 71, i32 25, i32 137, i32 31, i32 155,
	i32 86, i32 114, i32 147, i32 168, i32 121, i32 15, i32 43, i32 110,
	i32 80, i32 162, i32 118, i32 3, i32 102, i32 45, i32 142, i32 150,
	i32 79, i32 114, i32 157, i32 116, i32 165, i32 120, i32 5, i32 43,
	i32 107, i32 133, i32 52, i32 4, i32 151, i32 168, i32 112, i32 101,
	i32 37, i32 51, i32 152, i32 65, i32 119, i32 88, i32 82, i32 3,
	i32 124, i32 126, i32 9, i32 65, i32 150, i32 18, i32 55, i32 49,
	i32 82, i32 49, i32 92, i32 53, i32 2, i32 34, i32 28, i32 18,
	i32 14, i32 116, i32 11, i32 133, i32 58, i32 41, i32 96, i32 152,
	i32 17, i32 27, i32 85, i32 104, i32 7, i32 117, i32 25, i32 4,
	i32 103, i32 38, i32 17, i32 145, i32 47, i32 115, i32 146, i32 105,
	i32 118, i32 57, i32 99, i32 42, i32 87, i32 59, i32 166, i32 33,
	i32 75, i32 78, i32 125, i32 29, i32 32, i32 33, i32 41, i32 72,
	i32 161, i32 127, i32 54, i32 107, i32 167, i32 116, i32 61, i32 90,
	i32 121, i32 122, i32 9, i32 82, i32 162, i32 111, i32 56, i32 91,
	i32 10, i32 23, i32 105, i32 22, i32 21, i32 123, i32 39, i32 36,
	i32 128, i32 88, i32 52, i32 83, i32 158, i32 132, i32 1, i32 17,
	i32 128, i32 6, i32 13, i32 55, i32 102, i32 118, i32 111, i32 130,
	i32 50, i32 93, i32 16, i32 57, i32 62, i32 68, i32 74, i32 42,
	i32 19, i32 91, i32 87, i32 68, i32 72, i32 58, i32 69, i32 156,
	i32 101, i32 94, i32 129, i32 16, i32 120, i32 153, i32 145, i32 156,
	i32 71, i32 96, i32 84, i32 86, i32 12, i32 170, i32 102, i32 38,
	i32 46, i32 149, i32 135, i32 44, i32 5, i32 130, i32 152, i32 87,
	i32 23, i32 160, i32 19, i32 163, i32 117, i32 35, i32 140, i32 169,
	i32 146, i32 89, i32 70, i32 26, i32 159, i32 3, i32 70, i32 60,
	i32 62, i32 70, i32 40, i32 78, i32 61, i32 10, i32 0, i32 129,
	i32 46, i32 109, i32 124, i32 26, i32 168, i32 110, i32 22, i32 15,
	i32 115, i32 141, i32 153, i32 135, i32 100, i32 77, i32 73, i32 76,
	i32 134, i32 0, i32 119, i32 132, i32 74, i32 15, i32 170, i32 100,
	i32 143, i32 89, i32 117, i32 139, i32 92, i32 149, i32 147, i32 163,
	i32 114, i32 37, i32 28, i32 20, i32 63, i32 23, i32 36, i32 159,
	i32 107, i32 108, i32 32, i32 139, i32 97, i32 121, i32 148, i32 86,
	i32 150, i32 76, i32 147, i32 45, i32 14, i32 44, i32 89, i32 67,
	i32 47, i32 96, i32 97, i32 42, i32 46, i32 98, i32 35, i32 73,
	i32 43, i32 113, i32 30, i32 30, i32 132, i32 131, i32 45, i32 155,
	i32 157, i32 32, i32 11, i32 109, i32 75, i32 80, i32 129, i32 151,
	i32 148, i32 140, i32 139, i32 13, i32 170, i32 21, i32 104, i32 12,
	i32 7, i32 154, i32 172, i32 34, i32 72, i32 109, i32 57, i32 125,
	i32 98, i32 141, i32 133, i32 93, i32 79, i32 54, i32 74, i32 6,
	i32 136, i32 142, i32 52, i32 126, i32 19, i32 101, i32 166, i32 66,
	i32 131, i32 59, i32 128, i32 142, i32 80, i32 4, i32 145, i32 31,
	i32 56, i32 29, i32 131, i32 95, i32 163, i32 59, i32 10, i32 48,
	i32 140, i32 0, i32 161, i32 68, i32 58, i32 34, i32 135, i32 153,
	i32 92, i32 84, i32 85, i32 143, i32 37, i32 28, i32 134
], align 4

@marshal_methods_number_of_classes = dso_local local_unnamed_addr constant i32 0, align 4

@marshal_methods_class_cache = dso_local local_unnamed_addr global [0 x %struct.MarshalMethodsManagedClass] zeroinitializer, align 8

; Names of classes in which marshal methods reside
@mm_class_names = dso_local local_unnamed_addr constant [0 x ptr] zeroinitializer, align 8

@mm_method_names = dso_local local_unnamed_addr constant [1 x %struct.MarshalMethodName] [
	%struct.MarshalMethodName {
		i64 u0x0000000000000000, ; name: 
		ptr @.MarshalMethodName.0_name; char* name
	} ; 0
], align 8

; get_function_pointer (uint32_t mono_image_index, uint32_t class_index, uint32_t method_token, void*& target_ptr)
@get_function_pointer = internal dso_local unnamed_addr global ptr null, align 8

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
	store ptr %fn, ptr @get_function_pointer, align 8, !tbaa !3
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
attributes #0 = { memory(write, argmem: none, inaccessiblemem: none) "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" "target-cpu"="generic" "target-features"="+fix-cortex-a53-835769,+neon,+outline-atomics,+v8a" uwtable willreturn }
attributes #1 = { nofree nounwind }
attributes #2 = { noreturn "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" "target-cpu"="generic" "target-features"="+fix-cortex-a53-835769,+neon,+outline-atomics,+v8a" }

; Metadata
!llvm.module.flags = !{!0, !1, !7, !8, !9, !10}
!0 = !{i32 1, !"wchar_size", i32 4}
!1 = !{i32 7, !"PIC Level", i32 2}
!llvm.ident = !{!2}
!2 = !{!".NET for Android remotes/origin/release/9.0.1xx @ 1dcfb6f8779c33b6f768c996495cb90ecd729329"}
!3 = !{!4, !4, i64 0}
!4 = !{!"any pointer", !5, i64 0}
!5 = !{!"omnipotent char", !6, i64 0}
!6 = !{!"Simple C++ TBAA"}
!7 = !{i32 1, !"branch-target-enforcement", i32 0}
!8 = !{i32 1, !"sign-return-address", i32 0}
!9 = !{i32 1, !"sign-return-address-all", i32 0}
!10 = !{i32 1, !"sign-return-address-with-bkey", i32 0}
