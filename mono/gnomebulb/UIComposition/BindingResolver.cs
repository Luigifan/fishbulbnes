using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gtk;
using System.ComponentModel;

namespace UIComposition
{
	static class BindingResolver
	{
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
            Type type = dataContext.GetType();

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
