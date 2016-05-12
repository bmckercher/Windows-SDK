namespace MASFoundation
{
    /// <summary>
    /// HTTP request body type
    /// </summary>
    public enum RequestType
    {
        /// <summary>
        /// No request type specified
        /// </summary>
        None,

        /// <summary>
        /// JSON format
        /// </summary>
        Json,

        /// <summary>
        /// SCIM JSON format
        /// </summary>
        ScimJson,

        /// <summary>
        ///  Plain text
        /// </summary>
        PlainText,

        /// <summary>
        /// URL encoded
        /// </summary>
        FormUrlEncoded,

        /// <summary>
        /// XML format
        /// </summary>
        Xml
    }
}
