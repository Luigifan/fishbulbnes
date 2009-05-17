using System;
namespace UIComposition.BindingHandlers
{
    interface IPropertyToPropertyBindingDefinition
    {
        object Source { get; }
        string SourcePropertyName { get; }
        object Target { get; }
        string TargetPropertyName { get; }

        void TargetToSource();
        void SourceToTarget();
    }
}
