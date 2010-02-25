using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Collections;
using System.Reflection;
using System.Resources;
using System.Collections.ObjectModel;
using System.Globalization;


namespace InstiBulb.Converters
{

    /// <summary>
    /// code derived from http://www.ageektrapped.com/blog/the-missing-net-7-displaying-enums-in-wpf/
    /// </summary>
    public class EnumDisplayer : IValueConverter
    {
        private Type type;
        private Type resourceType;
        private object resources;
        private IDictionary displayValues;
        private IDictionary reverseValues;

        public EnumDisplayer()
        {
        }

        public ReadOnlyCollection<string> DisplayNames
        {
            get
            {
                Type displayValuesType = typeof(Dictionary<,>)
                                            .GetGenericTypeDefinition().MakeGenericType(type, typeof(string));
                this.displayValues = (IDictionary)Activator.CreateInstance(displayValuesType);

                this.reverseValues =
                   (IDictionary)Activator.CreateInstance(typeof(Dictionary<,>)
                            .GetGenericTypeDefinition()
                            .MakeGenericType( typeof(string), type));

                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
                foreach (var field in fields)
                {
                    DisplayTextAttribute[] a = (DisplayTextAttribute[])
                                                field.GetCustomAttributes(typeof(DisplayTextAttribute), false);

                    string displayString = GetDisplayStringValue(a);
                    object enumValue = field.GetValue(null);

                    if (displayString == null)
                    {
                        displayString = "undefined display string";
                    }
                    if (displayString != null)
                    {
                        displayValues.Add(enumValue, displayString);
                        reverseValues.Add(displayString, enumValue);
                    }
                }
                return new List<string>((IEnumerable<string>)displayValues.Values).AsReadOnly();
            }
        }

        private string GetDisplayStringValue(DisplayTextAttribute[] a)
        {
            if (a == null || a.Length == 0) return null;
            DisplayTextAttribute dsa = a[0];
            if (!string.IsNullOrEmpty(dsa.ResourceKey))
            {
                var fld = resourceType.GetProperty(dsa.ResourceKey, BindingFlags.NonPublic | BindingFlags.Static);
                string s = fld.GetValue(resources, null) as string;
                return s;

            }
            return dsa.Value;
        }

        public EnumDisplayer(Type type, Type resourceType)
        {
            this.Type = type;
            this.resourceType = resourceType;
            resources = Activator.CreateInstance(resourceType);
        }

        public Type Type
        {
            get { return type; }
            set
            {
                if (!value.IsEnum)
                    throw new ArgumentException("parameter is not an Enumermated type", "value");
                this.type = value;
            }
        }

        public Type ResourceType
        {
            get { return resourceType; }
            set
            {
                this.resourceType = value;
                resources = Activator.CreateInstance(resourceType, true);
            }
        }


        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return displayValues[value];
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return reverseValues[value];
        }
    }
}
