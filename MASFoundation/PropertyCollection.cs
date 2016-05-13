using MASFoundation.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASFoundation
{
    /// <summary>
    /// A collection of name / value pairs
    /// </summary>
    public sealed class PropertyCollection
    {
        /// <summary>
        /// Add a new property to the collection
        /// </summary>
        /// <param name="key">Key of the property</param>
        /// <param name="value">Value of the property</param>
        public void Add(string key, string value)
        {
            _properties.Add(new Property()
            {
                Key = key,
                Value = value
            });
        }

        /// <summary>
        /// Remove a property from the collection
        /// </summary>
        /// <param name="key">Key of the property to remove</param>
        public void Remove(string key)
        {
            PropertyList foundIndices = new PropertyList();
            for (var i = 0; i < _properties.Count; i++)
            {
                var pair = _properties[i];

                if (pair.Key == key)
                {
                    foundIndices.Add(pair);
                }
            }

            foundIndices.ForEach(pair => _properties.Remove(pair));
        }

        /// <summary>
        /// Get a property value from the collection
        /// </summary>
        /// <param name="key">Key of the property to retrieve</param>
        /// <returns>Value of property</returns>
        public string Get(string key)
        {           
            var found = _properties.FirstOrDefault(pair => pair.Key == key);
            return found?.Value;
        }

        /// <summary>
        /// Get a property from the collection based on its index
        /// </summary>
        /// <param name="index">Index of property</param>
        /// <returns>Value of property</returns>
        public Property GetAt(int index)
        {
            return _properties[index];
        }

        /// <summary>
        /// Number of properties
        /// </summary>
        public int Count
        {
            get
            {
                return _properties.Count;
            }
        }

        internal PropertyList Properties
        {
            get
            {
                return _properties;
            }
        }

        /// <summary>
        /// Property Collection as a string
        /// </summary>
        /// <returns>String representation</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var property in _properties)
            {
                sb.Append("\"{0}\": \"{1}\", ");
            }

            if (sb.Length > 2)
            {
                sb.Remove(sb.Length - 2, 2);
            }

            return sb.ToString();
        }

        PropertyList _properties = new PropertyList();
    }
}
