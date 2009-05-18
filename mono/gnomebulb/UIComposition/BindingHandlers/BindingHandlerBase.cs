using System.ComponentModel;
using System;
using Gtk;
using UIComposition.BindingHandlers;

namespace Gnomebulb.UIComposition.BindingHandlers
{
    internal class PropertyToPropertyBindingDefinition<T, S> : IPropertyToPropertyBindingDefinition
    {
        internal PropertyDescriptor targetProperty;
        internal T target;
        internal PropertyDescriptor sourceProperty;
        internal S source;

        string sourcePropertyName, targetPropertyName;

        public PropertyToPropertyBindingDefinition()
        { }

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
                (target as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(TargetPropertyChanged);
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

        public virtual void SourceToTarget()
        {
            
            object val = sourceProperty.GetValue(source);
            try
            {
                if (targetProperty.PropertyType == typeof(string))
                {
                    targetProperty.SetValue(target, val.ToString());
                }
                else
                {
                    targetProperty.SetValue(target, val);
                }

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

        public object Target
        {
            get { return target; }
        }

        public object Source
        {
            get { return source; }
        }

        public string SourcePropertyName
        {
            get { return sourcePropertyName; }

        }

        public string TargetPropertyName
        {
            get { return targetPropertyName; }

        }
    }

    class CheckButtonToBooleanBindingDefinition : PropertyToPropertyBindingDefinition<Gtk.CheckButton, object>
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
            sourceProperty.SetValue(source, target.Active);
        }
    }

    class ScaleBindingDefinition : PropertyToPropertyBindingDefinition<Gtk.Scale, object>
    {

        public ScaleBindingDefinition(object source, string sourcePropertyName, Scale target, string targetPropertyName)
            : base(source, sourcePropertyName, target, targetPropertyName)
        {

        }

        protected override void Initialize()
        {
            base.target.ValueChanged += new EventHandler(UpdateScaleValue);
            //+= new EventHandler(TargetPressEvent);	
        }
        void UpdateScaleValue(object o, EventArgs e)
        {
            TargetToSource();
        }

        public override void TargetToSource()
        {
            
            sourceProperty.SetValue(source, (Single)target.Value);
        }
    }

}