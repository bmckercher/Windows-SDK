/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

﻿using Windows.Data.Json;

namespace MASFoundation.Internal.Data
{
    static internal class JsonExtensions
    {
        public static string GetStringOrNull(this JsonObject obj, string key)
        {
            IJsonValue value;
            if (obj.TryGetValue(key, out value))
            {
                if (value.ValueType == JsonValueType.String)
                {
                    return value.GetString();
                }
            }

            return null;
        }

        public static IJsonValue ToJsonValue(this string text)
        {
            return text != null ? JsonValue.CreateStringValue(text) : JsonValue.CreateNullValue();
        }

        public static IJsonValue GetValueFromPath(this IJsonValue obj, string path)
        {
            var names = path.Split('.');

            IJsonValue foundValue = obj;
            foreach (var name in names)
            {
                if (foundValue.ValueType != JsonValueType.Object)
                {
                    foundValue = null;
                    break;
                }

                var parentObj = foundValue.GetObject();

                var arrayStartPos = name.IndexOf('[');

                if (arrayStartPos > 0)
                {
                    foundValue = null;
                    var arrayEndPos = name.IndexOf(']', arrayStartPos);

                    if (arrayEndPos > 0)
                    {
                        var arrayName = name.Substring(0, arrayStartPos);
                        var arrayIndexText = name.Substring(arrayStartPos + 1, arrayEndPos - arrayStartPos - 1);

                        uint arrayIndex;
                        if (parentObj.ContainsKey(arrayName) && uint.TryParse(arrayIndexText, out arrayIndex))
                        {
                            var foundArray = parentObj.GetNamedArray(arrayName);
                            if (arrayIndex < foundArray.Count)
                            {
                                foundValue = foundArray.GetObjectAt(arrayIndex);
                            }
                        }
                    }

                    if (foundValue == null)
                    {
                        break;
                    }
                }
                else
                {
                    if (parentObj.ContainsKey(name))
                    {
                        foundValue = parentObj.GetNamedValue(name);
                    }
                    else
                    {
                        foundValue = null;
                        break;
                    }
                }
            }

            return foundValue;
        }
    }
}
