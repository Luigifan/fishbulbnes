using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;

namespace PatientProfile
{

    public class UIInstigator<T>
    {

        public delegate void AddChildElementDelegate(T parent, T child, int row, int col);
        public delegate void BindViewModelDelegate(T view, IProfileViewModel model);
        public delegate T FindChildDelegate(T parent, string name);

        IUnityContainer container;

        public UIInstigator(IUnityContainer container,
                                AddChildElementDelegate addChild,
                                BindViewModelDelegate bindViewModel,
                                FindChildDelegate findChild
            )
        {
            this.addChild = addChild;
            this.bindViewModel = bindViewModel;
            this.findChild = findChild;
            
            this.container = container;

        }

        AddChildElementDelegate addChild;
        BindViewModelDelegate bindViewModel;
        FindChildDelegate findChild;

        public void Bootstrap(T host, IEnumerable<IProfileViewModel> views)
        {
            foreach (IProfileViewModel viewModel in views)
            {
                T control = BuildControl(viewModel.CurrentView);

                if (control == null)
                {
                    control = BuildControl("Default");
                }

                string[] pane = viewModel.CurrentRegion.Split(new char[] { '.' });
                int paneRowIndex = int.Parse(pane[1]);
                int paneColIndex = 0;
                if (pane.Length > 2)
                {
                    paneColIndex = int.Parse(pane[2]);
                }

                T g = findChild(host, pane[0]);
                if (g != null)
                {
                    addChild(g, control, paneRowIndex, paneColIndex);
                    bindViewModel(control, viewModel);

//                    if (viewModel.ChildViewModels != null)
//                    {
//                        Bootstrap(control, viewModel.ChildViewModels);
//                    }
                } else 
				{
					Console.WriteLine(pane[0] + " could not be resolved on " + host.ToString());	
				}
            }
        }

        private T BuildControl(string name)
        {
            T elem;
            try
            {
                elem = container.Resolve<T>(name);
            }
            catch
            {
                elem = default(T);
            }
            return elem;
        }

    }
}
