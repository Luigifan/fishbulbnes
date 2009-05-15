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
        class PropertyToPropertyBindingDefinition
        {

            PropertyDescriptor targetProperty;
            object target;
            PropertyDescriptor sourceProperty;
            object source;

            string sourcePropertyName, targetPropertyName;

            public PropertyToPropertyBindingDefinition(object source, string sourcePropertyName, object target, string targetPropertyName)
            {
                this.source = source;
                this.sourcePropertyName = sourcePropertyName;
                this.targetPropertyName = targetPropertyName;

                sourceProperty = TypeDescriptor.GetProperties(source)[sourcePropertyName];

                if (source is INotifyPropertyChanged)
                {
                    (source as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(SourcePropertyChanged);
                }

                this.target = target;
                if (target is INotifyPropertyChanged)
                {
                    (target as INotifyPropertyChanged).PropertyChanged +=new PropertyChangedEventHandler(TargetPropertyChanged);
                }
                targetProperty = TypeDescriptor.GetProperties(target)[targetPropertyName];
            }

            void SourcePropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == sourcePropertyName)
                {
                    SourceToTarget();
                }
            }

            void TargetPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == targetPropertyName)
                {
                    TargetToSource();
                }
            }

            public void SourceToTarget()
            {
                object val = sourceProperty.GetValue(source);
                try
                {
                    targetProperty.SetValue(target, val);
                    if (target is Widget)
                    {
                        Gtk.Application.Invoke( 
                            (o, e) => 
                        (target as Widget).QueueDraw()
                        );
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error updating binding: " + e.ToString());
                }
            }

            public void TargetToSource()
            {
                object val = targetProperty.GetValue(source);
                try
                {
                    sourceProperty.SetValue(target, val);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error updating binding: " + e.ToString());
                }
            }

        }

        static List<PropertyToPropertyBindingDefinition> currentBindings;

        static BindingResolver()
        {
            currentBindings = new List<PropertyToPropertyBindingDefinition>();
        }

        public static void CreateBinding(this Widget target, string targetPropertyName, object source, string sourcePropertyName)
        {
            currentBindings.Add(new PropertyToPropertyBindingDefinition(source, sourcePropertyName, target, targetPropertyName));
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
