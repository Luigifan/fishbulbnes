using Microsoft.Practices.Unity;
using Gtk;
using GtkNes;
using System;
using System.Linq;
using Fishbulb.Common.UI;
using UIComposition;

namespace TestGtkInstigation
{
	
	
	public class GTKInstigator : UIInstigator<Widget>
	{
		static void AddChildHandler(Widget parent, Widget child, int row, int col)
		{
			Gtk.Container c = parent as Container;
			if (c!= null)
			{
				child.Show();
				
				c.Add(child);
				c.ShowAll();
				Console.WriteLine("Child " + child.ToString() + " added: AddCHildHandler parent of type " + parent.GetType().ToString());
				
			} else {
				Console.WriteLine("AddCHildHandler parent of type " + parent.GetType().ToString());
			}
		}
		
		static Widget FindChild(Widget parent, string childName)
		{
			Gtk.Container c = parent as Container;
			if (c!= null)
			{
				if (c.Name == childName) return c;
				//if (c.Child == childName)return c.Child;
				foreach (Widget w in c.Children)
				{
					if (w.Name == childName) return w;
				}
				return null;
				
			}
			return null;
		}
		
		static void BindChild(Widget parent, IViewModel viewModel)
		{
			IBindableElement element = parent as IBindableElement;
			if (element != null)
			{
				element.DataContext = viewModel;
			}
		}
		
		
		public GTKInstigator(IUnityContainer container) : base(container, AddChildHandler, BindChild, FindChild  )
		{
		    		
		}

        public override void EndBootstrap()
        {
            BindingResolver.AllBindingsSourceToTarget();
        }

        
	}
}
