using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Gtk;

namespace Gnomebulb.UIComposition.BindingHandlers
{
	internal class TreeViewToListBindingDefinition : PropertyToPropertyBindingDefinition<Gtk.TreeView, object>
	{
        public TreeViewToListBindingDefinition(object source, string sourcePropertyName, Gtk.TreeView target, string targetPropertyName)
            : base(source, sourcePropertyName, target, targetPropertyName)
        {

        }
        Gtk.ListStore list = new Gtk.ListStore(typeof(string));
        protected override void Initialize()
        {
            TreeViewColumn tree = new TreeViewColumn();
            tree.Title = SourcePropertyName;
            target.AppendColumn(tree);
            Gtk.CellRendererText valueCell = new Gtk.CellRendererText();

            tree.PackStart(valueCell, true);

            tree.AddAttribute(valueCell, "text", 0);

            //+= new EventHandler(TargetPressEvent);	
        }

        public override void SourceToTarget()
        {
			if (sourceProperty == null) return;
			
			try{
	            list = new Gtk.ListStore(typeof(string));
	            IEnumerable<string> vals = sourceProperty.GetValue(source) as IEnumerable<string>;
	            if (vals != null)
	            {
	                foreach (string s in vals)
	                {
	                    list.AppendValues(s);
	                }
	                target.Model = list;
	            }
			} catch (Exception e)
			{
				Console.WriteLine("ListBinding.SourceToTarget " + e.ToString());
			}
        }

	}
}
