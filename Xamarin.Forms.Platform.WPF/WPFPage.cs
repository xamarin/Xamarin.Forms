using MahApps.Metro.Controls;
using System;
using System.ComponentModel;
using WPage=System.Windows.Controls.Page;

namespace Xamarin.Forms.Platform.WPF
{
	public class WPFPage : MetroWindow
	{
	    public WPFPage()
	    {
			
	    }

	    public event EventHandler<CancelEventArgs> BackKeyPress;

	    protected virtual bool OnBackKeyPress()
	    {
			var cancel= new CancelEventArgs();
			BackKeyPress?.Invoke(this, cancel);
	        return cancel.Cancel;
	    }

	    public virtual void SetIconMode(TitleIconMode titleIconMode)
	    {
	        
	    }
	}
}
