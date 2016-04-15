# Introduction #

UIInstigator is a simple library to perform composition of UIs based on the MVP/MVVM design pattern, by wiring ViewModels/Presenters to their corresponding Views dynamically.  The instigator is platform agnostic, it simply enforces a design pattern for GUI composition.

# Details #

The goal of the Instigator is to move as much common code as possible out of the specific UI library used to realize the application for a given platform.  It does so by forcing the MVVM view model, and by taking over the composition of the user interface.  Interaction between the ViewModel and View is completely done by data binding, and invisible to the user.

This is being created inline with fishbulbnes specifically to allow the richer interaction pieces (debugger, etc) to be easily realized on multiple platforms - currently Gtk#, WPF and Silverlight, however this list can and will likely grow.

# Implementation #
The instigator is based on the IOC development pattern, and is designed to be used with a IOC container.  The idea is that the GUI of the application will register all of it's available Views in the container, and then pass the container to the constructor of the Instigator.

# Bootstrapping #

After the Instigator has been created, the user calls the Bootstrap method, passing a reference to a top level ui element, and an array of IViewModel's, which are defined in the business application.

The bootstrap method then queries each IViewModel for it's desired View, which is resolved by name from the container.

The bootstrap method then queries the IViewModels desired "region", which refers to an area of interface space somewhere in the top level element passed to the bootstrap process.  The newly created element is placed in this region, and is then data bound to the ViewModel.


# Data Binding #

Since data binding of some sort is required, a simple library to perform WPF-like data binding in gtk# is being created to properly implement this design pattern on mono platforms.