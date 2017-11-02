package md5e0b39f3110588a210eb662a25619a1e5;


public class BitmapWorkerTask
	extends android.os.AsyncTask
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_doInBackground:([Ljava/lang/Object;)Ljava/lang/Object;:GetDoInBackground_arrayLjava_lang_Object_Handler\n" +
			"n_onPostExecute:(Ljava/lang/Object;)V:GetOnPostExecute_Ljava_lang_Object_Handler\n" +
			"";
		mono.android.Runtime.register ("ToDoXamarinDemo.Droid.BitmapWorkerTask, ToDoXamarinDemo.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", BitmapWorkerTask.class, __md_methods);
	}


	public BitmapWorkerTask ()
	{
		super ();
		if (getClass () == BitmapWorkerTask.class)
			mono.android.TypeManager.Activate ("ToDoXamarinDemo.Droid.BitmapWorkerTask, ToDoXamarinDemo.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public BitmapWorkerTask (android.content.ContentResolver p0, android.net.Uri p1)
	{
		super ();
		if (getClass () == BitmapWorkerTask.class)
			mono.android.TypeManager.Activate ("ToDoXamarinDemo.Droid.BitmapWorkerTask, ToDoXamarinDemo.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.ContentResolver, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:Android.Net.Uri, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0, p1 });
	}


	public java.lang.Object doInBackground (java.lang.Object[] p0)
	{
		return n_doInBackground (p0);
	}

	private native java.lang.Object n_doInBackground (java.lang.Object[] p0);


	public void onPostExecute (java.lang.Object p0)
	{
		n_onPostExecute (p0);
	}

	private native void n_onPostExecute (java.lang.Object p0);

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
