package crc64b2579bf00f2f8278;


public class UsbSerialPortInfo_UsbSerialPortAdapter
	extends android.widget.ArrayAdapter
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("SFKBle_Admin.UsbSerialPortInfo+UsbSerialPortAdapter, SFKBle_Admin", UsbSerialPortInfo_UsbSerialPortAdapter.class, __md_methods);
	}

	public UsbSerialPortInfo_UsbSerialPortAdapter (android.content.Context p0, int p1)
	{
		super (p0, p1);
		if (getClass () == UsbSerialPortInfo_UsbSerialPortAdapter.class) {
			mono.android.TypeManager.Activate ("SFKBle_Admin.UsbSerialPortInfo+UsbSerialPortAdapter, SFKBle_Admin", "Android.Content.Context, Mono.Android:System.Int32, System.Private.CoreLib", this, new java.lang.Object[] { p0, p1 });
		}
	}

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
