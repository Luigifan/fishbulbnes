using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gtk;
using System.ComponentModel;
using Gnomebulb.UIComposition.BindingHandlers;
using UIComposition.BindingHandlers;
using Fishbulb.Common.UI;

namespace UIComposition
{

	static class BindingResolver
	{

        static List<IPropertyToPropertyBindingDefinition> currentBindings;

        static BindingResolver()
        {
            currentBindings = new List<IPropertyToPropertyBindingDefinition>();
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

        public static void CreateTreeViewBinding<T>(this TreeView target, string targetPropertyName, object source, string sourcePropertyName)
        {
            if (target is Gtk.TreeView)
            {
                currentBindings.Add(new TreeViewToListBindingDefinition<T>(source, sourcePropertyName, target as TreeView, targetPropertyName));
            }
        }

        public static IEnumerable<T> QueryBindings<T>()
        {
            return from T binding in currentBindings where binding.GetType() == typeof(T) select binding;
        }

        public static void AllBindingsSourceToTarget()
        {
            foreach (IPropertyToPropertyBindingDefinition binding in currentBindings)
            {
                binding.SourceToTarget();
            }
        }

        public static void ExecuteCommand(this IViewModel model, string CommandName, object param)
        {
            if (model.Commands.ContainsKey(CommandName))
            {
                model.Commands[CommandName].Execute(param);
            }
        }
	}
}
