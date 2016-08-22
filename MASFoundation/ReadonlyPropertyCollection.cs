/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

﻿using MASFoundation.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASFoundation
{
    /// <summary>
    /// Readonly property collection
    /// </summary>
    public sealed class ReadonlyPropertyCollection
    {
        internal ReadonlyPropertyCollection(IDictionary<string, string> dictionary)
        {
            _properties = new PropertyList();

            foreach (var pair in dictionary)
            {
                _properties.Add(new Property()
                {
                    Key = pair.Key,
                    Value = pair.Value
                });
            }
        }

        internal ReadonlyPropertyCollection(PropertyList properties)
        {
            _properties = properties;
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

        /// <summary>
        /// Property Collection as a string
        /// </summary>
        /// <returns>String representation</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var property in _properties)
            {
                sb.AppendFormat("\"{0}\": \"{1}\", ", property.Key, property.Value);
            }

            if (sb.Length > 2)
            {
                sb.Remove(sb.Length - 2, 2);
            }

            return sb.ToString();
        }

        internal PropertyList Properties
        {
            get
            {
                return _properties;
            }
        }

        PropertyList _properties;
    }
}
