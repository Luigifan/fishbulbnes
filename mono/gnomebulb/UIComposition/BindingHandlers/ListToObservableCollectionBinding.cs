using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Gtk;

namespace Gnomebulb.UIComposition.BindingHandlers
{
	internal class TreeViewToListBindingDefinition : PropertyToPropertyBindingDefinition<Gtk.TreeView, IEnumerable<string>>
	{
        public TreeViewToListBindingDefinition(IEnumerable<string> source, string sourcePropertyName, Gtk.TreeView target, string targetPropertyName)
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
            list = new Gtk.ListStore(typeof(string));
            foreach (string s in source)
            {
                list.AppendValues(s);
            }
            target.Model = list;
        }

        public override void TargetToSource()
        {
            
            // sourceProperty.SetValue(source, (Single)target.Value);  
        }
	}
}
