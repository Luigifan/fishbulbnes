using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gtk;
using System.ComponentModel;
using Gnomebulb.UIComposition.BindingHandlers;

namespace UIComposition
{

	static class BindingResolver
	{

        static List<object> currentBindings;

        static BindingResolver()
        {
            currentBindings = new List<object>();
        }

        /// <summary>
        /// factory method for creating bindings
        /// </summary>
        /// <param name="target">the widget the binding will be created on</param>
        /// <param name="targetPropertyName">the target property name on the widget</param>
        /// <param name="source"></param>
        /// <param name="sourcePropertyName"></param>
        public static void CreateBinding(this Widget target, string targetPropertyName, object source, string sourcePropertyName)
        {
			Console.WriteLine(String.Format("CreateBinding {0} {1}", target.Name, target.GetType().ToString()));
			if (target is Gtk.CheckButton)
			{
            	currentBindings.Add(new CheckButtonToBooleanBindingDefinition(source , sourcePropertyName, target as CheckButton, targetPropertyName));
            }
            else if (target is Gtk.Scale)
            {
                currentBindings.Add(new ScaleBindingDefinition(source, sourcePropertyName, target as Scale, targetPropertyName));
            }
            else
            {
                currentBindings.Add(new PropertyToPropertyBindingDefinition<Widget, object>(source, sourcePropertyName, target, targetPropertyName));
            }
        }

        public static void UpdateBinding(this Widget target, string targetPropertyName)
        {

        }

        public static void UpdateSourceBinding(this Widget target, object dataContext, object value, string binding)
        {
            Type type = dataContext.GetType();

            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(dataContext)[binding];
            try
            {
                descriptor.SetValue(dataContext, value);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static object GetBindingValue(this Widget target, object dataContext, string binding)
        {
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(dataContext)[binding];
            try
            {
                return descriptor.GetValue(dataContext);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return null;
        }
	}
}
