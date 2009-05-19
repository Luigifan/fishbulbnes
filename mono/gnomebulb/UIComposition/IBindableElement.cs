
using System;
using System.ComponentModel;
using Fishbulb.Common.UI;
namespace GtkNes
{
	
	
	public interface IBindableElement
	{
		IViewModel DataContext { get; set; }
		
	}
}
