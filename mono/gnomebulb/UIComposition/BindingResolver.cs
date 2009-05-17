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
		internal class PropertyToPropertyBindingDefinition<T, S>
		{
            internal PropertyDescriptor targetProperty;
            internal T target;
            internal PropertyDescriptor sourceProperty;
            internal S source;

            string sourcePropertyName, targetPropertyName;
			
			public PropertyToPropertyBindingDefinition()
			{}
			
            public PropertyToPropertyBindingDefinition(S source, string sourcePropertyName, T target, string targetPropertyName)
            {
				Console.WriteLine(
                  string.Format("PropertyToPropertyBindingDefinition {0}.{1} {2}.{3}", source, sourcePropertyName, target, targetPropertyName));

				
                this.source = source;
                this.sourcePropertyName = sourcePropertyName;
                this.targetPropertyName = targetPropertyName;

                sourceProperty = TypeDescriptor.GetProperties(source)[sourcePropertyName];

                if (source is INotifyPropertyChanged)
                {
                    (source as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(SourcePropertyChanged);
                }
				
				Console.WriteLine("sourceProperty: " + sourceProperty);

                this.target = target;
                if (target is INotifyPropertyChanged)
                {
                    (target as INotifyPropertyChanged).PropertyChanged +=new PropertyChangedEventHandler(TargetPropertyChanged);
                }
                targetProperty = TypeDescriptor.GetProperties(target)[targetPropertyName];
				
				Console.WriteLine("targetProperty: " + sourceProperty);
				Initialize();
            }
			
			protected virtual void Initialize()
			{
				
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

            public virtual void TargetToSource()
            {
                try
                {
	                object val = targetProperty.GetValue(target);
                    sourceProperty.SetValue(source, val);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error updating binding: " + e.ToString());
                }
            }

			
		}
		
		class CheckButtonToBooleanBindingDefinition :  PropertyToPropertyBindingDefinition<Gtk.CheckButton, object>
		{
			protected override void Initialize()
			{
				 base.target.Toggled += TargetPressEvent;
					//+= new EventHandler(TargetPressEvent);	
			}

            public CheckButtonToBooleanBindingDefinition(object source, string sourcePropertyName, CheckButton target, string targetPropertyName) 
				: base(source, sourcePropertyName, target, targetPropertyName)
   		    {
				
			}

			void TargetPressEvent(object o, EventArgs e)
			{
				this.TargetToSource();
			}
			
			public override void TargetToSource()
			{
				//if (target.Mode
                    sourceProperty.SetValue(source, target.Mode);
			}
		}
		
		
		
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
                try
                {
	                object val = targetProperty.GetValue(target);
                    sourceProperty.SetValue(source, val);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error updating binding: " + e.ToString());
                }
            }

        }

        static List<object> currentBindings;

        static BindingResolver()
        {
            currentBindings = new List<object>();
        }

        public static void CreateBinding(this Widget target, string targetPropertyName, object source, string sourcePropertyName)
        {
			Console.WriteLine(String.Format("CreateBinding {0} {1}", target.Name, target.GetType().ToString()));
			if (target is Gtk.CheckButton)
			{
            	currentBindings.Add(new CheckButtonToBooleanBindingDefinition(source , sourcePropertyName, target as CheckButton, targetPropertyName));
				
			}else
			{
				
            	currentBindings.Add(new PropertyToPropertyBindingDefinition(source, sourcePropertyName, target, targetPropertyName));
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
