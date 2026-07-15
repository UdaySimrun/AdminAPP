package crc64b2579bf00f2f8278;


public class MainActivity_WindowInsetsListener
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.view.View.OnApplyWindowInsetsListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onApplyWindowInsets:(Landroid/view/View;Landroid/view/WindowInsets;)Landroid/view/WindowInsets;:GetOnApplyWindowInsets_Landroid_view_View_Landroid_view_WindowInsets_Handler:Android.Views.View/IOnApplyWindowInsetsListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
		mono.android.Runtime.register ("SFKBle_Admin.MainActivity+WindowInsetsListener, SFKBle_Admin", MainActivity_WindowInsetsListener.class, __md_methods);
	}

	public MainActivity_WindowInsetsListener ()
	{
		super ();
		if (getClass () == MainActivity_WindowInsetsListener.class) {
			mono.android.TypeManager.Activate ("SFKBle_Admin.MainActivity+WindowInsetsListener, SFKBle_Admin", "", this, new java.lang.Object[] {  });
		}
	}

	public android.view.WindowInsets onApplyWindowInsets (android.view.View p0, android.view.WindowInsets p1)
	{
		return n_onApplyWindowInsets (p0, p1);
	}

	private native android.view.WindowInsets n_onApplyWindowInsets (android.view.View p0, android.view.WindowInsets p1);

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
