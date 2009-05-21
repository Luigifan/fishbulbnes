using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Gtk;

namespace Gnomebulb.UIComposition.BindingHandlers
{
	internal class TreeViewToListBindingDefinition<BINDTYPE> : PropertyToPropertyBindingDefinition<Gtk.TreeView, object>
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

            Application.Invoke((o,e)=>RebuildSourceList());
            
        }

        private void RebuildSourceList()
        {
            try
            {

                list.Clear();
                var val = sourceProperty.GetValue(source);
                if (val != null)
                {
                    if (val is IEnumerable<BINDTYPE>)
                    {
                        if (typeof(BINDTYPE) == typeof(string))
                        {
							Console.WriteLine("Bindtype: " + typeof(BINDTYPE).ToString());
                            foreach (var o in val as IEnumerable<BINDTYPE>)
                            {
                                list.AppendValues(o);
                            }
                        }
                        else
                        {
							Console.WriteLine("Bindtype: " + typeof(BINDTYPE).ToString());
                            foreach (object o in val as IEnumerable<BINDTYPE>)
                            {
                                list.AppendValues(o.ToString());
								Console.WriteLine("Adding value: " + o.ToString());
                            }
                        }
                        target.Model = list;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ListBinding.SourceToTarget " + e.ToString());
            }
        }

	}
}
