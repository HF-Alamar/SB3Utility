/*
 * Copyright 2006 Sony Computer Entertainment Inc.
 * 
 * Licensed under the SCEA Shared Source License, Version 1.0 (the "License"); you may not use this 
 * file except in compliance with the License. You may obtain a copy of the License at:
 * http://research.scea.com/scea_shared_source_license.html
 *
 * Unless required by applicable law or agreed to in writing, software distributed under the License 
 * is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or 
 * implied. See the License for the specific language governing permissions and limitations under the 
 * License.
 * 
 * Modifications:
 *   Added Document.animations field
 *   Added reading of animations in Document(string)
 *   Added Animation, Sampler, and Channel clases
 *   Added channel option to Locator
 *   bindShapeMatrix was made public
 *   Document.baseURI was made public
 *   Primitive class made to support a list of <p> child nodes
 *   Removed throwing an exception if a visual scene doesn't have a node
 *   Added reading of Asset
 */

#region Using Statements
using System;
using System.Xml;                   // XmlDocument
using System.IO;                    // Console
using System.Globalization;         // CultureInfo
using System.Collections;
using System.Collections.Generic;   // List

using System.Text.RegularExpressions; // Regex

#endregion


namespace COLLADA
{
    // Summary:
    //     Represents a COLLADA document

    [Serializable()]
    public class Document
    {
        protected bool initialized = false;
        [NonSerialized()]
        protected XmlNamespaceManager nsmgr = null;
        [NonSerialized()]
        protected XmlDocument colladaDocument = null;
        //[NonSerialized()]
        public Uri baseURI;
        protected string documentName;
        protected string filename;
        public Hashtable dic;
        [NonSerialized()]
        protected CultureInfo encoding;

        // Helper functions

        public IColorOrTexture ColorOrTexture(XmlNode node)
        {
            // only one child !
            XmlNode child = node.FirstChild;
            switch (child.Name)
            {
                case "color":
                    return new Color(this, child);
                case "param":
                    return new ParamRef(this, child);
                case "texture":
                    return new Texture(this, child);
                default:
                    throw new Exception("un-expected node <" + child.Name + " in color_or_texture_type :" + filename);
            }
        }
        public IFloatOrParam FloatOrParam(XmlNode node)
        {
            // only one child !
            XmlNode child = node.FirstChild;
            switch (child.Name)
            {
                case "float":
                    return new Float(this, child);
                case "param":
                    return new ParamRef(this, child);
                default:
                    throw new Exception("un-expected node <" + child.Name + " in float_or_param_type :" + filename);
 
            }
        }
        public ITransparent TransparentParam(XmlNode node)
        {
            // only one child !
            XmlNode child = node.FirstChild;
            switch (child.Name)
            {
                case "color":
                    return new TransparentColor(this, child);
                case "param":
                    return new TransparentParamRef(this, child);
                case "texture":
                    return new TransparentTexture(this, child);
                default:
                    throw new Exception("un-expected node <" + child.Name + " in transparent parameters :" + filename);
            }
        }

        public T Get<T>(XmlNode node, string param, T defaultValue)
        {
            if (node.Attributes == null) return defaultValue;
            XmlAttribute attrib = node.Attributes[param];
            if (attrib != null) return (T)System.Convert.ChangeType(attrib.Value, typeof(T), encoding);
            else return defaultValue;
        }
        public T[] GetArray<T>(XmlNode node)
        {
            string[] stringValues = Regex.Split(node.InnerText, "[\\s]+");
            int i = 0;
            int k = stringValues.Length;
            if (stringValues[k - 1] == "") k -= 1;
            if (stringValues[0] == "") { i = 1; k--; }
            T[] p = new T[k];

            for (int j = 0; j < k; j++, i++)
                p[j] = (T)System.Convert.ChangeType(stringValues[i], typeof(T), encoding);
            return p;
        }


        public int sIndex=0;
        public void ResetIndex()
        {
            sIndex = 0;
        }
        
        /// <summary>
        /// Represents the COLLADA &lt;asset&gt; element.
        /// </summary>
		[Serializable()]
        public class Asset
        {
            public class Contributor
            {
                public string author;
                public string authoring_tool;
                public string comments;
                public string copyright;
                public string source_data;
                public Contributor() { }
                public Contributor(Document doc, XmlNode node)
                {
                    foreach (XmlNode child in node.ChildNodes)
                    {
                        switch (child.Name)
                        {
                            case "author":
                                author = child.InnerText;
                                break;
                            case "authoring_tool":
                                authoring_tool = child.InnerText;
                                break;
                            case "comments":
                                comments = child.InnerText;
                                break;
                            case "copyright":
                                copyright = child.InnerText;
                                break;
                            case "source_data":
                                source_data = child.InnerText;
                                break;
                            default:
                                throw new Exception("un-expected <" + child.Name + "> in <asset><contributor> :" + doc.filename);
                        }
                    }

                }
            } // end class Contributor

            public List<Contributor> contributors;
            // TODO: this is a date
            public string created;
            public string keywords;
            // TODO: this is a date
            public string modified;
            public string revision;
            public string subject;
            public string title;
            public string unit = "meter";
            public float meter = 1.0f;
            public string up_axis = "Y_UP";

            private Asset() { }
            public Asset(Document doc, XmlNode node)
            {
                
                foreach (XmlNode child in node.ChildNodes)
                {
                    switch (child.Name)
                    {
                        case "contributor":
                            if (contributors == null) contributors = new List<Contributor>();
                            contributors.Add(new Contributor(doc, child));
                            break;
                        case "created":
                            created = child.InnerText;
                            break;
                        case "modified":
                            modified = child.InnerText;
                            break;
                        case "keywords":
                            keywords = child.InnerText;
                            break;
                        case "revision":
                            revision = child.InnerText;
                            break;
                        case "subject":
                            subject = child.InnerText;
                            break;
                        case "title":
                            title = child.InnerText;
                            break;
                        case "unit":
                            meter = doc.Get<float>(child, "meter", 1.0f);
                            unit = doc.Get<string>(child, "name", null);
                            break;
                        case "up_axis":
                            up_axis = child.InnerText;
                            break;
                        default:
                            throw new Exception("un-expected node <" + child.Name + "> in asset :" + doc.filename);
                    }
                }
            }

        }
        /// <summary>
        /// This is a base class shared by a lot of elements.
        /// It contains the id, name and asset information that is contained by many COLLADA elements
        /// </summary>
        [Serializable()]
        public class Element
        {
            public string id;
            public string name;
            public Asset asset; // note: some elements derive from element, but do *not* have an asset tag

            private Element() { }
            public Element(Document doc, XmlNode node)
            {
                // get id and name
                id = doc.Get<string>(node, "id", null);
                if (id != null)
                {
                    if (doc.dic.ContainsKey(id)) throw new Exception("<" + node.Name + "> has non unique id : " + doc.filename);
                    doc.dic.Add(id, this);
                }
                name = doc.Get<string>(node, "name", null);
                XmlNode child = node.SelectSingleNode("child::asset", doc.nsmgr);
                if (child != null)
                    asset = new Asset(doc, child);
                // NOTE: extra not incuded in Element
            }
        }
        /// <summary>
        /// Represents the COLLADA "&lt;extra&gt;" element.
        /// </summary>
        [Serializable()]
        public class Extra : Element
        {
            public string type;
            public string profile;
            public string value;

            public Extra(Document doc, XmlNode node)
                : base(doc, node)
            {
                type = doc.Get<string>(node, "type", null);
                // TODO: find only *immediate* children
                XmlNode child = node.SelectSingleNode("colladans:technique", doc.nsmgr);
                profile = doc.Get<string>(child, "profile", null);
                if (profile == null) throw new Exception("profile missing in <extra><technique>" + doc.filename);
                value = child.InnerText;
            }
        }
        /// <summary>
        /// Represents the COLLADA &lt;xxx_array&gt; elements, including float_array, int_array, Name_array....
        /// </summary>
        [Serializable()]
        public class Array<T> : Element
        {
            protected int count; // make it read only to avoid user errors
            public T[] arr;

            public int Count { get { return count; } }
            public T this[int i]
            {
                get { return arr[i]; }
                set { arr[i] = value; }
            }
            public Array(Document doc, XmlNode arrayElement)
                : base(doc, arrayElement)
            {

                if (id == null) throw new Exception("Array [" + arrayElement.Name + "has invalid id :" + doc.filename);
                count = doc.Get<int>(arrayElement, "count", 0);

                arr = new T[count];
                string[] stringValues = arrayElement.InnerText.Split(new Char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                int arrayCount = 0;
                for (int i = 0; i < stringValues.Length && arrayCount < count; i++)
                {
                    if (stringValues[i] != "")
                    {
                        arr[arrayCount++] = (T)System.Convert.ChangeType(stringValues[i], typeof(T), doc.encoding);
                    }
                    
                }
            }
        }
        /// <summary>
        /// Represents the COLLADA &lt;param&gt; element.
        /// </summary>
        [Serializable()]
        public class Param
        {
            public string name;
            public string sid;
            public string semantic;
            public string type;
            public string value;
            public int index;   // calculated when loading the document
            private Param() { }

            public Param(Document doc, XmlNode node)
            {
                index = doc.sIndex++;
                name = doc.Get<string>(node, "name", null);
                sid = doc.Get<string>(node, "sid", null);
                semantic = doc.Get<string>(node, "semantic", null);
                type = doc.Get<string>(node, "type", null);
                if (type == null) throw new Exception("missing type information on param " + node + " :" + doc.filename);
                value = node.InnerXml;
            }
        }
        /// <summary>
        /// Represents the COLLADA &lt;anotate&gt; element.
        /// </summary>
        [Serializable()]
        public class Annotate
        {
            private string name;
            private string type;
            private string value;
            private Annotate() { }
            public Annotate(Document doc, XmlNode node)
            {
                name = doc.Get<string>(node, "name", null);
                if (name == null) throw new Exception("missing name information on annotate " + node + " :" + doc.filename);
                XmlNode child = node.FirstChild;
                type = child.Name;
                value = child.InnerXml;
            }
        }       
        /// <summary>
        /// Represents the COLLADA &lt;samppler2D&gt; element.
        /// </summary>
        [Serializable()]
        public class Sampler2D : Sampler3D
        {
            // no wrapP in sampler2D

            public Sampler2D(Document doc, XmlNode node)
                : base(doc, node)
            {
            }
        }
        /// <summary>
        /// Represents the COLLADA &lt;samppler1D&gt; element.
        /// </summary>
        [Serializable()]
        public class Sampler1D : Sampler3D
        {
            // no wrapP, wrapT in sampler2D

            public Sampler1D(Document doc, XmlNode node)
                : base(doc, node)
            {
            }
        }
        public interface IFxBasicTypeCommon { } ;
        /// <summary>
        /// Represents the COLLADA &lt;samppler3D&gt; element.
        /// </summary>
        [Serializable()]
        public class Sampler3D : IFxBasicTypeCommon
        {
            public string source = null;
            public string wrapS = "WRAP";
            public string wrapT = "WRAP";
            public string wrapP = "WRAP";
            public string minFilter = "NONE";
            public string magFilter = "NONE";
            public string mipFilter = "NONE";
            public string borderColor = null;
            public uint mipmapMaxlevel = 255;
            public float mipmapBias = 0.0f;
            List<Extra> extras;

            private Sampler3D() {}

            public Sampler3D(Document doc, XmlNode node)
            {
                foreach (XmlNode child in node.ChildNodes)
                {
                    switch (child.Name)
                    {
                        case "source":
                            source = child.InnerText;
                            break;
                        case "wrap_s":
                            wrapS = child.InnerText;
                            break;
                        case "wrap_t":
                            wrapT = child.InnerText;
                            break;
                        case "wrap_p":
                            wrapP = child.InnerText;
                            break;
                        case "minfilter":
                            minFilter = child.InnerText;
                            break;
                        case "magfilter":
                            magFilter = child.InnerText;
                            break;
                        case "mipfilter":
                            mipFilter = child.InnerText;
                            break;
                        case "border_color":
                            borderColor = child.InnerText;
                            break;
                        case "mipmap_maxlevel":
                            mipmapMaxlevel = uint.Parse(child.InnerText, doc.encoding);
                            break;
                        case "mipmap_bias":
                            mipmapBias = float.Parse(child.InnerText, doc.encoding);
                            break;
                        case "extra":
                            if (extras == null) extras = new List<Extra>();
                            extras.Add(new Extra(doc, child));
                            break;

                        default:
                            throw new Exception(child.Name + " is not supported in " + this.ToString());
                    }
                }
               
                
            }
        }
        /// <summary>
        /// Represents the COLLADA &lt;surface&gt; element.
        /// </summary>
        [Serializable()]
        public class Surface : IFxBasicTypeCommon
        {
            public string type;
            public string initFrom;
            public string format;
            private Surface() { }
            public Surface(Document doc, XmlNode node)
            {
                foreach (XmlNode child in node.ChildNodes)
                {
                    switch (child.Name)
                    {
                        case "type":
                            type = child.InnerText;
                            break;
                        case "init_from":
                            initFrom = child.InnerText;
                            break;
                        case "format":
                            format = child.InnerText;
                            break;
                        default:
                            throw new Exception(child.Name + " is not recognized in <surface>");
                    }
                }
            }
        }
        /// <summary>
        /// Represents the COLLADA &lt;new_param&gt; element.
        /// </summary>
        [Serializable()]
        public class NewParam
        {
            public string sid;
            private List<Annotate> annotates;
            public string semantic;
            // modifier enum...
            public string modifier;
            public IFxBasicTypeCommon param;

            private NewParam() { }
            public NewParam(Document doc, XmlNode node)
            {
                sid = doc.Get<string>(node, "sid", null);
                if (sid == null) throw new Exception("sid is required in newparam" + node + " :" + doc.filename);


                foreach (XmlNode child in node.ChildNodes)
                {
                    switch (child.Name)
                    {
                        case "semantic":
                            semantic = child.InnerXml;
                            break;
                        case "modifier":
                            modifier = child.InnerXml;
                            break;
                        case "annotate":
                            if (annotates == null) annotates = new List<Annotate>();
                            annotates.Add(new Annotate(doc, child));
                            break;
                        case "surface":
                            param = new Surface(doc,child);
                            break;
                        case "sampler1D":
                            param = new Sampler1D(doc, child);
                            break;
                        case "sampler2D":
                            param = new Sampler2D(doc, child);
                            break;
                        case "sampler3D":
                            param = new Sampler3D(doc, child);
                            break;
                        default:
                            throw new Exception(child.Name + " is not supported yet in NewParam");
                    }
                }
            }
        }
        /// <summary>
        /// Represents the COLLADA &lt;accessor&gt; element.
        /// </summary>
        [Serializable()]
        public class Accessor
        {
            public int count = -1;
            public int offset = -1;
            public Locator source;
            public int stride = -1;
            public List<Param> parameters;
            private Accessor() { }
            public Accessor(Document doc, XmlNode node)
            {
                count = doc.Get<int>(node, "count", -1);
                if (count < 0) throw new Exception("invalid count in accessor");
                offset = doc.Get<int>(node, "offset", 0);
                source = new Locator(doc, node);

                stride = doc.Get<int>(node, "stride", 1);

                
                doc.ResetIndex();  // index used by param list
                foreach (XmlNode paramElement in node.ChildNodes)
                {
                    if (paramElement.Name != "param") throw new Exception("Invalid element <" + paramElement.Name + "> in <acessor> :" + doc.filename);
                    if (parameters == null) parameters = new List<Param>();
                    parameters.Add(new Param(doc, paramElement));
                }
            }
        }
        public interface ISourceOrVertices
        {
        }
        /// <summary>
        /// Represents the COLLADA &lt;source&gt; element.
        /// </summary>
        [Serializable()]
        public class Source : Element, ISourceOrVertices
        {

            public object array;
            public string arrayType;
            public Accessor accessor;
            public Source(Document doc, XmlNode node)
                : base(doc, node)
            {

                if (id == null) throw new Exception("Source[" + node.Name + "] does not have id ! : " + doc.filename);
                // Read a source
                // TODO - test if it has a unique array
                foreach (XmlNode child in node.ChildNodes)
                {
                    switch (child.Name)
                    {
                        case "float_array":
                            array = new Array<float>(doc, child);
                            arrayType = child.Name;
                            break;
                        case "int_array":
                            array = new Array<int>(doc, child);
                            arrayType = child.Name;
                            break;
                        case "bool_array":
                            array = new Array<bool>(doc, child);
                            arrayType = child.Name;
                            break;
                        case "IDREF_array":
                        case "Name_array":
                            array = new Array<string>(doc, child);
                            arrayType = child.Name;
                            break;
                        case "technique":
                            break;
                        case "technique_common":
                            XmlNode accessorElement = child.FirstChild;
                            if (accessorElement == null || accessorElement.Name != "accessor") throw new Exception("expected <accessor> in <technique_common> in <mesh><source>");
                            accessor = new Accessor(doc, accessorElement);
                            break;
                        default:
                            throw new Exception("Un recognized array : " + child.Name);
                    }
                }

            }
        }
        /// <summary>
        /// Represents the COLLADA &lt;input&gt; element.
        /// </summary>
        [Serializable()]
        public class Input
        {
            public int offset;
            public string semantic;
            public ISourceOrVertices source;
            public int set;
            private Input() {}
            public Input(Document doc, XmlNode node)
            {
                semantic = doc.Get<string>(node, "semantic", "");
                if (semantic == "") throw new Exception("input has no semantic");
                offset = doc.Get<int>(node, "offset", -1);
                set = doc.Get<int>(node, "set", -1);  // need to keep this a int if want to use negative values for special meaning
                Locator loc = new Locator(doc,node);
                source = (ISourceOrVertices)doc.dic[loc.Fragment];
            }
        }
        /// <summary>
        /// Represents the COLLADA &lt;vertices&gt; element.
        /// </summary>
        [Serializable()]
        public class Vertices : Element, ISourceOrVertices
        {
            public List<Extra> extras;
            public List<Input> inputs;

            //private vertices() { }
            public Vertices(Document doc, XmlNode node)
                : base(doc, node)
            {

                if (id == null) throw new Exception("Vertices[" + node.Name + "] does not have id ! : " + doc.filename);
               
                // Read inputs
                XmlNodeList inputElements = node.SelectNodes("colladans:input", doc.nsmgr);
                if (inputElements.Count != 0) inputs = new List<Input>();
                foreach (XmlNode inputElement in inputElements)
                {
                    inputs.Add(new Input(doc, inputElement));
                }
                // Get Extras
                XmlNodeList extraElements = node.SelectNodes("colladans:extra", doc.nsmgr);
                if (extraElements.Count != 0) extras = new List<Extra>();
                foreach (XmlNode extraElement in extraElements)
                {
                    extras.Add(new Extra(doc, extraElement));
                }
            }
        }
        /// <summary>
        /// Locator is used to store all the URI values one can find in a COLLADA document
        /// </summary>
        [Serializable()]
        public class Locator
        {
            private bool isFragment = false;
            private bool isRelative = false;
            private bool isInvalid = true;
            private Uri url;
            private Locator() { }
            public Locator(Document doc, XmlNode node)
            {
                string path = null;
                if (node.Name == "init_from" || node.Name == "skeleton")
                {
                    path = node.InnerXml;
                }
                else if (node.Name == "instance_material" || node.Name == "bind" )
                {
                    path = doc.Get<string>(node, "target", null);
                }
                else if (node.Name == "input" || node.Name == "accessor" || node.Name == "skin" || node.Name == "morph" || node.Name == "channel")
                {
                    path = doc.Get<string>(node, "source", null);
                }
                else
                {
                    path = doc.Get<string>(node, "url", null);
                }

                if (path == null || path == "") return;

                if (path.StartsWith("#")) // fragment URI
                {
                    //string relative_path = documentName + path;
                    url = new Uri(doc.baseURI, path);
                    isFragment = true;
                }
                else if (path.Contains(":")) // full uri
                {
                    url = new Uri(path);
                }
                else // relative URI or erronous URI
                {
                    url = new Uri(doc.baseURI, path);
                    isRelative = true;
                }
                isInvalid = false;
                //Console.WriteLine("Found uri = " + url);
            }
            public bool IsFragment { get { return isFragment; } }
            public bool IsRelative { get { return isRelative; } }
            public bool IsInvalid { get { return isInvalid; } }
            public string Fragment
            {
                get
                {
                    if (!isFragment) throw new Exception("cannot get Fragment of a non Fragment URI" + this.ToString() + this.url.ToString());
                    else return url.Fragment.Substring(1); 
                
                }
            }
            public Uri Uri
            {
                get { return url; }
            }
        }
        public interface IColorOrTexture
        {
            // common_color_or_texture_type
            // float or param or texture
        }
        public interface IFloatOrParam
        {
            // common_float_or_param_type
            // float or param
        }
        public interface ITransparent
        {
            // common_color_or_texture_type - float or param or texture
            // + opaque - string(Default A_ONE)
        }
        /// <summary>
        /// Represents the COLLADA &lt;color&gt; element.
        /// </summary>
        [Serializable()]
        public class Color : IColorOrTexture
        {
            public string sid;
            public float[] floats;
            private Color() { }
            public Color(Document doc, XmlNode node)
            {
                sid = doc.Get<string>(node, "sid", null);
                floats = doc.GetArray<float>(node);
            }
            public float this[int i]
            {
                get { return floats[i]; }
            }
        }
        [Serializable()]
        public class TransparentFloat : Float, ITransparent
        {   
            public string opaque;
            public TransparentFloat(Document doc, XmlNode node)
                : base(doc, node) 
            { 
                opaque = doc.Get<string>(node, "opaque", "A_ONE");
            }
        }
        [Serializable()]
        public class TransparentTexture : Texture, ITransparent
        {
            public string opaque;
            public TransparentTexture(Document doc, XmlNode node)
                : base(doc, node)
            {
                opaque = doc.Get<string>(node, "opaque", "A_ONE");
            }
        }
        [Serializable()]
        public class TransparentParamRef : ParamRef, ITransparent
        {
            public string opaque;
            public TransparentParamRef(Document doc, XmlNode node)
                : base(doc, node)
            {
                opaque = doc.Get<string>(node, "opaque", "A_ONE");
            }
        }
        [Serializable()]
        public class TransparentColor : Color, ITransparent
        {
            public string opaque;
            public TransparentColor(Document doc, XmlNode node) : base(doc , node) 
            { 
                opaque = doc.Get<string>(node, "opaque", "A_ONE");
            }
        }
        [Serializable()]
        public class ParamRef : IColorOrTexture, IFloatOrParam
        {
            public string reference;
            private ParamRef() { }
            public ParamRef(Document doc, XmlNode node)
            {
                reference = doc.Get<string>(node, "ref", null);
                if (reference == null) throw new Exception("missing mandatory ref parameter in <profile_COMMON><technique><param> :" + doc.filename);
            }
        }
        [Serializable()]
        public class Texture : IColorOrTexture
        {
            public string texture;
            public string texcoord;
            public List<Extra> extras;
            private Texture() { }
            public Texture(Document doc, XmlNode node)
            {
                texture = doc.Get<string>(node, "texture", null);
                if (texture == null) throw new Exception("missing texture parameter in <profile_COMMON><technique><texture> :" + doc.filename);
                texcoord = doc.Get<string>(node, "texcoord", null);
                if (texcoord == null) throw new Exception("missing texcoord parameter in <profile_COMMON><technique><texture> :" + doc.filename);
                XmlNodeList extraElements = node.SelectNodes("colladans:extra", doc.nsmgr);
                if (extraElements.Count != 0) extras = new List<Extra>();
                foreach (XmlNode extraElement in extraElements) extras.Add(new Extra(doc, extraElement));
            }

        }

        [Serializable()]
        public class Float : IFloatOrParam
        {
            public string sid;
            public float theFloat;
            private Float() { }
            public Float(Document doc, XmlNode node)
            {
                sid = doc.Get<string>(node, "sid", null);
                theFloat = float.Parse(node.InnerText, doc.encoding);
            }
        }
        [Serializable()]
        public class SimpleShader
        {
            public IColorOrTexture emission;
            public IColorOrTexture ambient;
            public IColorOrTexture diffuse;
            public IColorOrTexture specular;
            public IFloatOrParam shininess;
            public IColorOrTexture reflective;
            public IFloatOrParam reflectivity;
            public ITransparent transparent;
            public IFloatOrParam transparency;
            public IFloatOrParam indexOfRefraction;
            private SimpleShader() { }
            public SimpleShader(Document doc, XmlNode node)
            {
              
                foreach (XmlNode child in node.ChildNodes)
                {
                    switch (child.Name)
                    {
                        case "emission":
                            emission = doc.ColorOrTexture(child);
                            break;
                        case "ambient":
                            ambient = doc.ColorOrTexture(child);
                            break;
                        case "diffuse":
                            diffuse = doc.ColorOrTexture(child);
                            break;
                        case "specular":
                            specular = doc.ColorOrTexture(child);
                            break;
                        case "shininess":
                            shininess = doc.FloatOrParam(child);
                            break;
                        case "reflective":
                            reflective = doc.ColorOrTexture(child);
                            break;
                        case "reflectivity":
                            reflectivity = doc.FloatOrParam(child);
                            break;
                        case "transparent":
                            transparent = doc.TransparentParam(child);
                            break;
                        case "transparency":
                            transparency = doc.FloatOrParam(child);
                            break;
                        case "index_of_refraction":
                            indexOfRefraction = doc.FloatOrParam(child);
                            break;
                        default:
                            throw new Exception("un expected node <" + child.Name + "> in <technique_COMMON><technique> :" + doc.filename);
                    }
                }
            }
        }
        /// <summary>
        /// Represents a COMMON profile constant shader.
        /// </summary>
        [Serializable()]
        public class Constant : SimpleShader
        {
            public Constant(Document doc, XmlNode node) : base(doc, node) { }
        }
        /// <summary>
        /// Represents a COMMON profile Lambert shader.
        /// </summary>
        [Serializable()]
        public class Lambert : SimpleShader
        {
            public Lambert(Document doc, XmlNode node) : base(doc, node) { }
        }
        /// <summary>
        /// Represents a COMMON profile Phong shader.
        /// </summary>
        [Serializable()]
        public class Phong : SimpleShader
        {
            public Phong(Document doc, XmlNode node) : base(doc, node) { }
        }
        /// <summary>
        /// Represents a COMMON profile Blinn shader.
        /// </summary>
        [Serializable()]
        public class Blinn : SimpleShader
        {
            public Blinn(Document doc, XmlNode node) : base(doc, node) { }
        }

        public interface IProfile { };
        /// <summary>
        /// Represents the COLLADA &lt;profile_COMMON&gt; element.
        /// </summary>
        [Serializable()]
        public class ProfileCOMMON : Element, IProfile   //    Note: this is missing the name attribute !
        {

            [Serializable()]
            public class Technique : Element
            {
                public string sid;
                public List<Image> images;
                public Dictionary<string, NewParam> newParams;
                public SimpleShader shader;
                //private technique() {}
                public Technique(Document doc, XmlNode node)
                    : base(doc, node)
                {

                    sid = doc.Get<string>(node, "sid", null);

                    foreach (XmlNode child in node.ChildNodes)
                    {
                        switch (child.Name)
                        {
                            case "image":
                                if (images == null) images = new List<Image>();
                                images.Add(new Image(doc, child));
                                break;
                            case "newparam":
                                NewParam tmpNewParam = new NewParam(doc, child);
                                if (newParams == null) newParams = new Dictionary<string, NewParam>();
                                newParams[tmpNewParam.sid] = tmpNewParam;
                                break;
                            case "asset":
                            case "extra":
                                break;
                            case "constant":
                                shader = new Constant(doc, child);
                                break;
                            case "lambert":
                                shader = new Lambert(doc, child);
                                break;
                            case "phong":
                                shader = new Phong(doc, child);
                                break;
                            case "blinn":
                                shader = new Blinn(doc, child);
                                break;
                            default:
                                throw new Exception("<profile_COMMON> <technique> un-expected" + child.Name);
                        }
                    }
                }
            }

            public Technique technique;

            public List<Image> images;
            public Dictionary<string,NewParam> newParams;
            public ProfileCOMMON(Document doc, XmlNode node)
                : base(doc, node)
            {

                images = new List<Image>();
                newParams = new Dictionary<string,NewParam>();
                foreach (XmlNode child in node.ChildNodes)
                {
                    switch (child.Name)
                    {
                        case "image":
                            images.Add(new Image(doc, child));
                            break;
                        case "technique":
                            technique = new Technique(doc, child);
                            break;
                        case "newparam":
                            NewParam tmpNewParam = new NewParam(doc, child);
                            newParams[tmpNewParam.sid] = tmpNewParam;
                            break;
                        case "asset":
                        case "extra":
                            break;
                        default:
                            throw new Exception("un-expected <" + child.Name + "> in profile_COMMON :" + doc.filename);
                    }
                }
            }
        }
        /// <summary>
        /// Represents the COLLADA &lt;effect&gt; element.
        /// </summary>
        [Serializable()]
        public class Effect : Element
        {
            public List<Annotate> annotates;
            public List<Image> images;
            public Dictionary<string,NewParam> newparams;
            public List<IProfile> profiles;

            public Locator instance_effect;

            public Effect(Document doc, XmlNode node)
                : base(doc, node)
            {
                if (id == null) throw new Exception("Effect[" + node.Name + "] does not have id ! : " + doc.filename);

                // TODO: there can be many profiles !, even common profiles !
                profiles = new List<IProfile>();
                XmlNode profileElement = node.SelectSingleNode("colladans:profile_COMMON", doc.nsmgr);
                if (profileElement == null) throw new Exception("effect id=" + id + " has no profile_COMMON :" + doc.filename);
                profiles.Add(new ProfileCOMMON(doc, profileElement));

                // get all images
                XmlNodeList imageElements = node.SelectNodes("colladans:image", doc.nsmgr);
                images = new List<Image>();
                foreach (XmlNode imageElement in imageElements)
                {
                    images.Add(new Image(doc, imageElement));
                }
                // get all newparams
                XmlNodeList newparamElements = node.SelectNodes("colladans:newparam", doc.nsmgr);
                newparams = new Dictionary<string,NewParam>();
                foreach (XmlNode newParamElement in newparamElements)
                {
                    NewParam tmpNewParam = new NewParam(doc, newParamElement);
                    newparams[tmpNewParam.sid] = tmpNewParam;
                }
                // get all annotate
                XmlNodeList annotateElements = node.SelectNodes("colladans:annotate", doc.nsmgr);
                annotates = new List<Annotate>();
                foreach (XmlNode annotateElement in annotateElements)
                {
                    annotates.Add(new Annotate(doc, annotateElement));
                }

            }
        }
        /// <summary>
        /// Represents the COLLADA &lt;material&gt; element.
        /// </summary>
        [Serializable()]
        public class Material : Element
        {
            public Locator instanceEffect;

            public Material(Document doc, XmlNode node)
                : base(doc, node)
            {
                if (id == null) throw new Exception("Material[" + node.Name + "] does not have id ! : " + doc.filename);
             
                XmlNode instance_effectElement = node.SelectSingleNode("colladans:instance_effect", doc.nsmgr);
                if (instance_effectElement == null) throw new Exception("Material[" + id + "] does not have <instance_effect> : " + doc.filename);
                instanceEffect = new Locator(doc, instance_effectElement);
                if (instanceEffect.IsInvalid) throw new Exception("Material[" + id + "] does not have url in <instance_effect> : " + doc.filename);
            }
        }
        /// <summary>
        /// base class used to represent all the COLLADA primitives (triangles, lines, polygons...
        /// </summary>
        [Serializable()]
        public class Primitive 
        {
            public string name;
            public string material;
            public int count;
            protected List<Input> inputs;
            public int stride = 0; // maximum offset + 1
            public int[] p;
            public int[] vcount;
            public List<Extra> extras;

            public List<Input> Inputs
            {
                get { return inputs; }
                set
                {
                    inputs = value;
                    stride = 0; // maximum offset + 1
                    foreach (Input input in inputs)
                        if (input.offset >= stride) stride = input.offset + 1;
                }
            }
            private Primitive () {}
            protected Primitive(Document doc, List<Input> _inputs, int[] _p)
            {
                p = _p;
                Inputs = _inputs;
            }
            public Primitive(Document doc, XmlNode node)
            {
                name = doc.Get<string>(node, "name", "");
                count = doc.Get<int>(node, "count", -1);
                if (count <= 0) throw new Exception("count <=0 in <triangle:");
                material = doc.Get<string>(node, "material", "");

                // Read <input> <p> and <extra>
                inputs = new List<Input> ();
				List<int[]> pList = new List<int[]>();
				int pLen = 0;
                foreach (XmlNode child in node.ChildNodes)
                {
                    switch (child.Name)
                    {
                        case "input":
                            Input input = new Input(doc, child);
                            inputs.Add(input);
                            if (input.offset >= stride) stride = input.offset + 1;
                            break;
                        case "p":
							int[] arr = doc.GetArray<int>(child);
							pList.Add(arr);
							pLen += arr.Length;
                            break;
                        case "vcount":
                            vcount = doc.GetArray<int>(child);
                            break;
                        case "extra":
                            if (extras == null) extras = new List<Extra>();
                            extras.Add(new Extra(doc, child));
                            break;
                        default:
                            throw new Exception("un-recognized element " + child.Name + " in triangle");
                    }
                }

				p = new int[pLen];
				int copyIdx = 0;
				foreach (int[] arr in pList)
				{
					arr.CopyTo(p, copyIdx);
					copyIdx += arr.Length;
				}
            }
        }
        /// <summary>
        /// Represents the COLLADA &lt;triangle&gt; element.
        /// </summary>
        [Serializable()]
        public class Triangle : Primitive
        {
            public Triangle(Document doc, XmlNode node) : base(doc,node) {}
            public Triangle(Document doc,  int _count, List<Input> _inputs, int[] _p)
                : base(doc, _inputs, _p)
            {
                count = _count;
            }
        }
        /// <summary>
        /// Represents the COLLADA &lt;line&gt; element.
        /// </summary>
        [Serializable()]
        public class Line : Primitive
        {
            public Line(Document doc, XmlNode node) : base(doc, node) { }
        }
        /// <summary>
        /// Represents the COLLADA &lt;polylist&gt; element.
        /// </summary>
        [Serializable()]
        public class Polylist : Primitive
        {
            public Polylist(Document doc, XmlNode node) : base(doc, node) { }
        }
        /// <summary>
        /// Represents the COLLADA &lt;mesh&gt; element.
        /// </summary>
        [Serializable()]
        public class Mesh
        {
            //get all the sources
            public List<Source> sources;
            public List<Primitive> primitives;
            public Vertices vertices;

            private Mesh() { }
            public Mesh(Document doc, XmlNode node)
            {

                foreach (XmlNode child in node.ChildNodes)
                {
                    switch (child.Name)
                    {
                        case "source":
                            if (sources == null) sources = new List<Source>();
                            sources.Add(new Source(doc, child));
                            break;
                        case "vertices":
                            vertices = new Vertices(doc, child);
                            break;
						case "polygons":
                        case "triangles":
                            if (primitives == null) primitives = new List<Primitive>();
                            primitives.Add(new Triangle(doc, child));
                            break;
                        case "polylist":
                            if (primitives == null) primitives = new List<Primitive>();
                            primitives.Add(new Polylist(doc, child));
                            break;
                        case "lines":
                            if (primitives == null) primitives = new List<Primitive>();
                            primitives.Add(new Line(doc, child));
                            break;
                        default:
                            throw new Exception(child.Name+" type not suported yet");
                    }
                }

                if (sources.Count == 0) throw new Exception("<mesh> does not contain a <source> : " + doc.filename);
                if (vertices == null) throw new Exception("<mesh> does not contain a <vertices> : " + doc.filename);
             
            }
              
        }
        /// <summary>
        /// Base class to represent all the possible transforms in COLLADA.
        /// </summary>
        [Serializable()]
        public class TransformNode
        {
            protected float[] floats;
            public string sid;
            private TransformNode() { }
            public TransformNode(Document doc, XmlNode node)
            {   
                sid = doc.Get<string>(node, "sid", null);
                floats = doc.GetArray<float>(node);
            }
            public float this[int i]
            {
                get { return floats[i]; }
                set { floats[i] = value; }
            }
            public int Size { get { return floats.Length; } }
        }
        /// <summary>
        /// Represents the COLLADA &lt;lookat&gt; element.
        /// </summary>
        [Serializable()]
        public class Lookat : TransformNode
        {
            public Lookat(Document doc, XmlNode node) : base(doc, node) { }
            public float this[int i, int j]
            {
                get { return floats[3 * i + j]; }
                set { floats[3 * i + j] = value; }
            }
        }
        /// <summary>
        /// Represents the COLLADA &lt;lmatrix&gt; element.
        /// </summary>
        [Serializable()]
        public class Matrix : TransformNode
        {
            public Matrix(Document doc, XmlNode node) : base(doc, node) { }
            public float this[int i, int j]
            {
                get { return floats[4 * i + j]; }
                set { floats[4 * i + j] = value; }
            }
        }
        /// <summary>
        /// Represents the COLLADA &lt;rotate&gt; element.
        /// </summary>
        [Serializable()]
        public class Rotate : TransformNode
        {
            public Rotate(Document doc, XmlNode node) : base(doc, node) { }
        }
        /// <summary>
        /// Represents the COLLADA &lt;scale&gt; element.
        /// </summary>
        [Serializable()]
        public class Scale : TransformNode
        {
            public Scale(Document doc, XmlNode node) : base(doc, node) { }
        }
        /// <summary>
        /// Represents the COLLADA &lt;translate&gt; element.
        /// </summary>
        [Serializable()]
        public class Translate : TransformNode
        {
            public Translate(Document doc, XmlNode node) : base(doc, node) { }
        }
        /// <summary>
        /// Represents the COLLADA &lt;skew&gt; element.
        /// </summary>
        [Serializable()]
        public class Skew : TransformNode
        {
            public Skew(Document doc, XmlNode node) : base(doc, node) { }
        }
        /// <summary>
        /// Base class to represent COLLADA instances.
        /// </summary>
        [Serializable()]
        public class Instance 
        {
            List<Extra> extras;
            public Locator url;
            public string sid;
            public string name;
            private Instance() { }
            public Instance(Document doc, XmlNode node)
            {
                url = new Locator(doc, node);
                if (url.IsInvalid) throw new Exception("missing url in " + node.Name + " in file:" + doc.filename);
                sid = doc.Get<string>(node, "sid", null);
                name = doc.Get<string>(node, "name", null);
                XmlNodeList extraElements = node.SelectNodes("colladans:extra", doc.nsmgr);
                if (extraElements.Count != 0) extras = new List<Extra>();
                foreach (XmlNode extraElement in extraElements) extras.Add(new Extra(doc, extraElement));
            }
        }

        [Serializable()]
        public class InstanceWithMaterialBind : Instance
        {
            public BindMaterial bindMaterial;
            public InstanceWithMaterialBind(Document doc, XmlNode node)
                : base(doc, node)
            {
                XmlNode bindMaterialElement = node.SelectSingleNode("colladans:bind_material", doc.nsmgr);
                if (bindMaterialElement != null)
                    bindMaterial = new BindMaterial(doc, bindMaterialElement);    
            }
        }
        /// <summary>
        /// Represents the COLLADA &lt;instance_camera&gt; element.
        /// </summary>
        [Serializable()]
        public class InstanceCamera : Instance
        {
            public InstanceCamera(Document doc, XmlNode node) : base(doc, node) { }  // constructor
        };
        /// <summary>
        /// Represents the COLLADA &lt;instance_controler&gt; element.
        /// </summary>
        [Serializable()]
        public class InstanceController : InstanceWithMaterialBind
        {
            public List<Locator> skeleton;

            public InstanceController(Document doc, XmlNode node)
                : base(doc, node)
            {

                foreach (XmlNode child in node.ChildNodes)
                {
                    switch (child.Name)
                    {
                        case "skeleton":
                            if (skeleton == null) skeleton = new List<Locator>();
                            skeleton.Add(new Locator(doc, child));
                            break;
                        case "bind_material":
                            // base class takes care of bind_material
                            break;
                        case "extra":
                            // Instance base class takes care of extra already
                            break;
                        default:
                            throw new Exception("unexpected <" + child.Name + "> in <instance_controler> :" + doc.filename);

                    }
                }
            } 
        };
        /// <summary>
        /// Represents the COLLADA &lt;instance_material&gt; element.
        /// </summary>
        [Serializable()]
        public class InstanceMaterial
        {
            public string symbol;
            public Locator target;
            public string sid;
            public string name;
            [Serializable()]
            public struct Bind
            {
                public string semantic;
                public Locator target;
            };
            public List<Bind> binds;
            [Serializable()]
            public struct BindVertexInput
            {
                public string semantic;
                public string inputSemantic;
                public uint inputSet;
            };
            public List<BindVertexInput> bindVertexInputs;

            public List<Extra> extras;
            private InstanceMaterial() { }
            public InstanceMaterial(Document doc, XmlNode node)
            {
                symbol = doc.Get<string>(node, "symbol", null);
                if (symbol == null) throw new Exception("missing symbol parameter in instance_material" + node + " :" + doc.filename);
                target = new Locator(doc, node);
                if (target == null) throw new Exception("missing target parameter in instance_material" + node + " :" + doc.filename);
                sid = doc.Get<string>(node, "sid", null);
                name = doc.Get<string>(node, "sid", null);

                foreach (XmlNode child in node.ChildNodes)
                {
                    switch (child.Name)
                    {
                        case "bind":
                            {
                                Bind tmp;
                                tmp.semantic = doc.Get<string>(child, "semantic", null);
                                if (tmp.semantic == null) throw new Exception("invalid semantic in <instance_material><bind> :" + doc.filename);
                                tmp.target = new Locator(doc, node);
                                if (tmp.target == null) throw new Exception("invalid target in <instance_material><bind> :" + doc.filename);
                                if (binds == null) binds = new List<Bind>();
                                binds.Add(tmp);
                            }
                            break;
                        case "bind_vertex_input":
                            {
                                BindVertexInput tmp;
                                tmp.semantic = doc.Get<string>(child, "semantic", null);
                                if (tmp.semantic == null) throw new Exception("invalid semantic in <instance_material><bind> :" + doc.filename);
                                tmp.inputSemantic = doc.Get<string>(child, "input_semantic", null);
                                if (tmp.inputSemantic == null) throw new Exception("invalid input_semantic in <instance_material><bind> :" + doc.filename);
                                tmp.inputSet = doc.Get<uint>(child, "input_set", 0);
                                if (bindVertexInputs == null) bindVertexInputs = new List<BindVertexInput>();
                                bindVertexInputs.Add(tmp);
                            }
                            break;
                        case "extra":
                            if (extras == null) extras = new List<Extra>();
                            extras.Add(new Extra(doc, child));
                            break;
                        default:
                            throw new Exception("un-expected node " + child.Name + " in <instance_material> :" + doc.filename);

                    }
                }
            }

        }
        /// <summary>
        /// Represents the COLLADA &lt;bind_material&gt; element.
        /// </summary>
        [Serializable()]
        public class BindMaterial : Element
        {
            public Dictionary<string, Param> parameters;
            public Dictionary<string,InstanceMaterial> instanceMaterials;
            //public List<technique
            public List<Extra> extras;

            public BindMaterial(Document doc, XmlNode node)
                : base(doc, node)
            {


                foreach (XmlNode child in node.ChildNodes)
                {
                    switch (child.Name)
                    {
                        case "param":
                            Param param = new Param(doc, child); 
                            if (parameters == null) parameters = new Dictionary<string,Param>();
                            parameters[param.name]=param;
                            break;
                        case "technique_common":
                            foreach (XmlNode temp in child.ChildNodes)
                            {
                                if (temp.Name != "instance_material") throw new Exception("illegal node <" + temp.Name + "> in <bind_material><technique_common> :" + doc.filename);
                                if (instanceMaterials == null) instanceMaterials = new Dictionary<string,InstanceMaterial>();
                                InstanceMaterial tmpInstanceMaterial = new InstanceMaterial(doc, temp);
                                instanceMaterials[tmpInstanceMaterial.target.Fragment] = tmpInstanceMaterial;
                            }
                            break;
                        case "technique":
                            break;
                        case "extra":
                            if (extras == null) extras = new List<Extra>();
                            extras.Add(new Extra(doc, child));
                            break;
                        default:
                            throw new Exception("un-expected node " + child.Name + " in <instance_material> :" + doc.filename);

                    }
                }

            }
        }
        /// <summary>
        /// Represents the COLLADA &lt;instance_geometry&gt; element.
        /// </summary>
        [Serializable()]
        public class InstanceGeometry : InstanceWithMaterialBind
        {
            public InstanceGeometry(Document doc, XmlNode node)
                : base(doc, node)
            {
                foreach (XmlNode child in node.ChildNodes)
                {
                    switch (child.Name)
                    {
                        case "bind_material":
                            // base class takes care of bind_material
                            break;
                        case "extra":
                            // Intance constructor take care of extra already
                            break;
                        default:
                            throw new Exception("unexpected <" + child.Name + "> in <instance_controler> :" + doc.filename);

                    }
                }
            } // constructor
        };
        /// <summary>
        /// Represents the COLLADA &lt;instance_light&gt; element.
        /// </summary>
        [Serializable()]
        public class InstanceLight : Instance
        {
            public InstanceLight(Document doc, XmlNode node) : base(doc, node) { } // constructor
        } ;
        /// <summary>
        /// Represents the COLLADA &lt;instance_node&gt; element.
        /// </summary>
        public class InstanceNode : Instance
        {
            public InstanceNode(Document doc, XmlNode node) : base(doc, node) { } // constructor
        } ;
        /// <summary>
        /// Represents the COLLADA &lt;node&gt; element.
        /// </summary>
        [Serializable()]
        public class Node : Element
        {
            public string sid;
            public string type;
            public string layer;
            public List<TransformNode> transforms;
            public List<Node> children;
            public List<Instance> instances;
            public List<Extra> extras;

            public Node(Document doc, XmlNode root)
                : base(doc, root)
            {

                sid = doc.Get<string>(root, "sid", null);
                type = doc.Get<string>(root, "type", "NODE");
                layer = doc.Get<string>(root, "layer", null);

                foreach (XmlNode childElement in root.ChildNodes)
                {
                    switch (childElement.Name)
                    {
                        case "lookat":
                            if (transforms == null) transforms = new List<TransformNode>();
                            transforms.Add(new Lookat(doc, childElement));
                            break;
                        case "matrix":
                            if (transforms == null) transforms = new List<TransformNode>();
                            transforms.Add(new Matrix(doc, childElement));
                            break;
                        case "rotate":
                            if (transforms == null) transforms = new List<TransformNode>();
                            transforms.Add(new Rotate(doc, childElement));
                            break;
                        case "scale":
                            if (transforms == null) transforms = new List<TransformNode>();
                            transforms.Add(new Scale(doc, childElement));
                            break;
                        case "skew":
                            if (transforms == null) transforms = new List<TransformNode>();
                            transforms.Add(new Skew(doc, childElement));
                            break;
                        case "translate":
                            if (transforms == null) transforms = new List<TransformNode>();
                            transforms.Add(new Translate(doc, childElement));
                            break;
                        case "instance_camera":
                            if (instances == null) instances = new List<Instance>();
                            instances.Add(new InstanceCamera(doc, childElement));
                            break;
                        case "instance_controller":
                            if (instances == null) instances = new List<Instance>();
                            instances.Add(new InstanceController(doc, childElement));
                            break;
                        case "instance_geometry":
                            if (instances == null) instances = new List<Instance>();
                            instances.Add(new InstanceGeometry(doc, childElement));
                            break;
                        case "instance_light":
                            if (instances == null) instances = new List<Instance>();
                            instances.Add(new InstanceLight(doc, childElement));
                            break;
                        case "instance_node":
                            if (instances == null) instances = new List<Instance>();
                            instances.Add(new InstanceNode(doc, childElement));
                            break;
                        case "node":
                            if (children == null) children = new List<Node>();
                            children.Add(new Node(doc, childElement)); // recursive call
                            break;
                        case "extra":
                            if (extras == null) extras = new List<Extra>();
                            extras.Add(new Extra(doc, childElement));
                            break;
                        default:
                            throw new Exception("invalid node[" + childElement.Name + "] in <node> in file " + doc.filename);
                    }
                }

            }
        }
        /// <summary>
        /// Represents the COLLADA &lt;visual_scene&gt; element.
        /// </summary>
        [Serializable()]
        public class VisualScene : Element
        {
            public List<Node> nodes;
            //private evaluate_scene List<>
            //private visual_scene() { }
            public VisualScene(Document doc, XmlNode node)
                : base(doc, node)
            {

                XmlNodeList nodeElements = node.SelectNodes("colladans:node", doc.nsmgr);
                //if (nodeElements.Count == 0) throw new Exception("visual_scene[" + id + "] does not contain a <node> : " + doc.filename);
                nodes = new List<Node>();
                foreach (XmlNode nodeElement in nodeElements)
                {
                    nodes.Add(new Node(doc, nodeElement));
                }
            }
        }        
        public interface ISkinOrMorph 
        {
            //Locator Source;
        }
        [Serializable()]
        public class Skin : ISkinOrMorph
        {
            public Locator source;
            public List<Source> sources;
            public struct Joint
            {
                public List<Input> inputs;
                public List<Extra> extras;
            }
            public Joint joint;
            public struct VertexWeights
            {
                public uint count;
                public List<Input> inputs;
                public uint[] vcount;
                public int[] v;
                public List<Extra> extras;
            }
            public VertexWeights vertexWeights;
            public List<Extra> extras;
            public Matrix bindShapeMatrix;

            public Skin(Document doc, XmlNode node)
            {
                source = new Locator(doc, node);

                foreach (XmlNode child in node.ChildNodes)
                {
                    switch (child.Name)
                    {
                        case "bind_shape_matrix":
                            bindShapeMatrix = new Matrix(doc,child); 
                            break;
                        case "source":
                            if (sources == null) sources = new List<Source>();
                            sources.Add(new Source(doc, child));
                            break;
                        case "joints":
                            // grab all the sub-elements
                            XmlNodeList inputElements = child.SelectNodes("colladans:input", doc.nsmgr);
                            if (inputElements.Count != 0)
                              joint.inputs = new List<Input>();
                            else
                                throw new Exception ("no <input> elements in <skin><joints>");
                            foreach (XmlNode inputElement in inputElements)
                            {
                                joint.inputs.Add(new Input(doc,inputElement));
                            }
                            // Do the same for EXTRA !
                            XmlNodeList extraElements = child.SelectNodes("colladans:extra", doc.nsmgr);
                            if (extraElements.Count != 0)
                                joint.extras = new List<Extra>();
                            foreach (XmlNode extraElement in extraElements)
                            {
                                joint.extras.Add(new Extra(doc, extraElement));
                            }
                            break;
                        case "vertex_weights":
                            vertexWeights.count = doc.Get<uint>(child, "count", 0);
                            // grab all the sub-elements
                            inputElements = child.SelectNodes("colladans:input", doc.nsmgr);
                            if (inputElements.Count >= 2)
                                vertexWeights.inputs = new List<Input>();
                            else
                                throw new Exception("need at least 2 <input> elements in <skin><vertex_weights>");
                            foreach (XmlNode inputElement in inputElements)
                            {
                                vertexWeights.inputs.Add(new Input(doc, inputElement));
                            }
                            // Do the same for EXTRA !
                            
                            extraElements = child.SelectNodes("colladans:extra", doc.nsmgr);
                            if (extraElements.Count != 0)
                                vertexWeights.extras = new List<Extra>();
                            foreach (XmlNode extraElement in extraElements)
                            {
                                vertexWeights.extras.Add(new Extra(doc, extraElement));
                            }
                            vertexWeights.vcount =  doc.GetArray<uint>(child.SelectSingleNode("colladans:vcount", doc.nsmgr));
                            vertexWeights.v = doc.GetArray<int>(child.SelectSingleNode("colladans:v", doc.nsmgr));
                            break;
                        case "extra":
                            if (extras == null) extras = new List<Extra>();
                            extras.Add(new Extra(doc,child));
                            break;
                        default:
                            throw new Exception("invalide node "+child.Name+ "in <Skin>");
                    }
                }
            }
        }
        /// <summary>
        /// Represents the COLLADA &lt;morph&gt; element.
        /// </summary>
        [Serializable()]
        public class Morph : ISkinOrMorph
        {
            Locator source;
            List<Source> sources;
            public string method;
            public class Target
            {
                public List<Input> inputs;
                public List<Extra> extras;
            }
            public Target target;
            public List<Extra> extras;
            public Morph(Document doc, XmlNode node)
            {
                source = new Locator(doc, node);
                method = doc.Get<string>(node, "method", "NORMALIZED");
                foreach (XmlNode child in node.ChildNodes)
                {
                    switch (child.Name)
                    {
                        case "source":
                            if (sources == null) sources = new List<Source>();
                            sources.Add(new Source(doc, child));
                            break;
                        case "targets":
                            // grab all the sub-elements
                            XmlNodeList inputElements = child.SelectNodes("colladans:input", doc.nsmgr);
                            if (inputElements.Count != 0)
                                target.inputs = new List<Input>();
                            else
                                throw new Exception("no <input> elements in <skin><joints>");
                            foreach (XmlNode inputElement in inputElements)
                            {
                                target.inputs.Add(new Input(doc, inputElement));
                            }
                            // Do the same for EXTRA !
                            XmlNodeList extraElements = child.SelectNodes("colladans:extra", doc.nsmgr);
                            if (extraElements.Count != 0)
                                target.extras = new List<Extra>();
                            foreach (XmlNode extraElement in extraElements)
                            {
                                target.extras.Add(new Extra(doc, extraElement));
                            }
                            break;
                        case "extra":
                            if (extras == null) extras = new List<Extra>();
                            extras.Add( new Extra(doc,child));
                            break;
                        default:
                            throw new Exception("invalide node " + child.Name + "in <Skin>");
                    }
                }
            }
        }
        /// <summary>
        /// Represents the COLLADA &lt;controller&gt; element.
        /// </summary>
        [Serializable()]
        public class Controller : Element
        {
            public ISkinOrMorph controller;
            public List<Extra> extras;
            public Controller(Document doc, XmlNode node)
                : base(doc, node)
            {
                if (id == null) throw new Exception("Controller[" + node.Name + "] does not have id ! : " + doc.filename);

                foreach (XmlNode child in node.ChildNodes)
                {
                    switch (child.Name)
                    {
                        case "skin":
                            controller = new Skin(doc, child);
                            break;
                        case "morph":
                            controller = new Morph(doc, child);
                            break;
                        case "extra":
                            if (extras == null) extras = new List<Extra>();
                            extras.Add(new Extra(doc,child));
                            break;
                        default:
                            throw new Exception ("Invalid node "+child.Name+" type in <controller>");
                    }
                }

            }
        }
        /// <summary>
        /// Represents the COLLADA &lt;geometry&gt; element.
        /// </summary>
        [Serializable()]
        public class Geometry : Element
        {
            public Mesh mesh;

            public Geometry(Document doc, XmlNode node)
                : base(doc, node)
            {

                if (id == null) throw new Exception("Geometry[" + node.Name + "] does not have id ! : " + doc.filename);


                // contains only one type of mesh
                XmlNode meshElement = node.SelectSingleNode("colladans:mesh", doc.nsmgr);
                if (meshElement == null) throw new Exception("Geometry[" + id + "] does not contain a <mesh> : " + doc.filename);

                // TODO: convex_mesh and spline

                // read mesh
                mesh = new Mesh(doc, meshElement);
            }
        }
        /// <summary>
        /// Represents the COLLADA &lt;image&gt; element.
        /// </summary>
        [Serializable()]
        public class Image : Element
        {
            public bool isData;
            public Locator init_from;
            public string data;
            public string format;
            public int height;
            public int width;
            public int depth;

            // private image() { }
            public Image(Document doc, XmlNode node)
                : base(doc, node)
            {
                if (id == null) throw new Exception("Image[" + node.Name + "] does not have id ! : " + doc.filename);

                format = doc.Get<string>(node, "format", "");
                height = doc.Get<int>(node, "height", -1);
                width = doc.Get<int>(node, "width", -1);
                depth = doc.Get<int>(node, "depth", -1);

                XmlNode dataElement = node.SelectSingleNode("colladans:data", doc.nsmgr);
                XmlNode init_fromElement = node.SelectSingleNode("colladans:init_from", doc.nsmgr);

                if (dataElement != null)
                {
                    isData = true;
                    // TODO: load image from DATA
                    data = dataElement.InnerXml;
                }
                else if (init_fromElement != null)
                {
                    isData = false;
                    init_from = new Locator(doc, init_fromElement);
                    if (init_from.IsInvalid) throw new Exception("<image> <init_from> is invalid URL :" + doc.filename);
                }
                else throw new Exception("Image[" + id + "] does not contain either <init_from> or <data>: " + doc.filename);
            }
        }
        /// <summary>
        /// Represents the COLLADA &lt;instance_scene&gt; element.
        /// </summary>
        [Serializable()]
        public class InstanceScene
        {
            public Locator url;
            public string sid;
            public string name;
            private InstanceScene() { }
            public InstanceScene(Document doc, XmlNode node)
            {
                url = new Locator(doc, node);
                if (url.IsInvalid) throw new Exception("instance_scene[" + node.Name + "] does not have url ! : " + doc.filename);
                sid = doc.Get<string>(node, "sid", null);
                name = doc.Get<string>(node, "sid", name);
            }
        }
        // public abstract T Import(string filename, ContentImporterContext context);

		/// <summary>
		/// Represents the COLLADA &lt;animation&gt; element.
		/// </summary>
		[Serializable()]
		public class Animation : Element
		{
			public List<Source> sources;
			public Sampler sampler;
			public Channel channel;
			public List<Animation> children;

			public Animation(Document doc, XmlNode node)
				: base(doc, node)
			{
				foreach (XmlNode child in node.ChildNodes)
				{
					switch (child.Name)
					{
						case "source":
							if (sources == null) { sources = new List<Source>(); }
							sources.Add(new Source(doc, child));
							break;
						case "sampler":
							sampler = new Sampler(doc, child);
							break;
						case "channel":
							channel = new Channel(doc, child);
							break;
						case "animation":
							if (children == null) { children = new List<Animation>(); }
							children.Add(new Animation(doc, child));
							break;
						default:
							throw new Exception(child.Name + " is not recognized in <animation>");
					}
				}
			}
		}

		/// <summary>
		/// Represents the COLLADA &lt;sampler&gt; element.
		/// </summary>
		[Serializable()]
		public class Sampler : Element
		{
			public List<Input> inputs;

			public Sampler(Document doc, XmlNode node)
				: base(doc, node)
			{
				if (id == null)
				{
					throw new Exception("Sampler[" + node.Name + "] doesn't have an id: " + doc.filename);
				}

				foreach (XmlNode child in node.ChildNodes)
				{
					switch (child.Name)
					{
						case "input":
							if (inputs == null) { inputs = new List<Input>(); }
							inputs.Add(new Input(doc, child));
							break;
						default:
							throw new Exception(child.Name + " is not recognized in <sampler>");
					}
				}
			}
		}
		/// <summary>
		/// Represents the COLLADA &lt;channel&gt; element.
		/// </summary>
		[Serializable()]
		public class Channel
		{
			public Locator source;
			public string target;

			private Channel() { }
			public Channel(Document doc, XmlNode node)
			{
				source = new Locator(doc, node);
				target = doc.Get<string>(node, "target", "");
			}
		}

        [NonSerialized()]
        private XmlNode root = null;
		public Asset asset;
        public List<Image> images;
        public List<Material> materials;
        public List<Effect> effects;
        public List<Geometry> geometries;
        public List<Controller> controllers;
        public List<Node> nodes;
        public List<VisualScene> visualScenes;
		public List<Animation> animations;
        public InstanceScene instanceVisualScene;
        public InstanceScene instancePhysicsScene;

        public Document()
        {

            dic = new Hashtable();
            dic.Clear();
            colladaDocument = new XmlDocument();
            nsmgr = new XmlNamespaceManager(colladaDocument.NameTable);
            nsmgr.AddNamespace("colladans", "http://www.collada.org/2005/11/COLLADASchema");
            encoding = new System.Globalization.CultureInfo("en-US");

        }
        /// <summary>
        /// Loads a COLLADA document from a file. Returns a Document object.
		/// </summary>
        /// <param name="name"> is the name of the file to be loaded </param>
        public Document(string name)
            : this()
        {
            filename = name;
            if (!File.Exists(filename))
                throw new FileNotFoundException("Could ot find file:" + filename);
            colladaDocument = new XmlDocument();
            colladaDocument.Load(filename);
            root = colladaDocument.DocumentElement;
            int split = colladaDocument.BaseURI.LastIndexOf("/");
            baseURI = new Uri(colladaDocument.BaseURI);
            documentName = colladaDocument.BaseURI.Substring(split + 1);
            // get encoding scheme rom xml document, default to en-US
            // TODO: test this !
            string culture = Get<string>(colladaDocument, "encoding", "en-US");
            encoding = new System.Globalization.CultureInfo(culture);

            // TODO: xmlns="http://www.collada.org/2005/11/COLLADASchema" version="1.4.1">
            // TODO: read axis and unit from <asset>

            // parse document for all libraries
			XmlNodeList assetNodeList = root.SelectNodes("colladans:asset", nsmgr);

			foreach (XmlNode assetNode in assetNodeList)
			{
				asset = new Asset(this, assetNode);
			}

            XmlNodeList imagesLibs = root.SelectNodes("colladans:library_images", nsmgr);
            
            foreach (XmlNode imagesLib in imagesLibs)
            {
                XmlNodeList imageElements = imagesLib.SelectNodes("colladans:image", nsmgr);
                foreach (XmlNode imageElement in imageElements)
                {
                    if (images == null) images = new List<Image>();
                    images.Add(new Image(this, imageElement));
                }
            }

            XmlNodeList materialsLibs = root.SelectNodes("colladans:library_materials", nsmgr);
            
            foreach (XmlNode materialsLib in materialsLibs)
            {
                XmlNodeList materialElements = materialsLib.SelectNodes("colladans:material", nsmgr);
                foreach (XmlNode materialElement in materialElements)
                {
                    if (materials == null) materials = new List<Material>();
                    materials.Add(new Material(this, materialElement));
                }
            }

            XmlNodeList effectsLibs = root.SelectNodes("colladans:library_effects", nsmgr);
            
            foreach (XmlNode effectsLib in effectsLibs)
            {
                XmlNodeList effectElements = effectsLib.SelectNodes("colladans:effect", nsmgr);
                foreach (XmlNode effectElement in effectElements)
                {
                    if (effects == null) effects = new List<Effect>();
                    effects.Add(new Effect(this, effectElement));
                }
            }

            XmlNodeList geometryLibs = root.SelectNodes("colladans:library_geometries", nsmgr);
            
            foreach (XmlNode geometryLib in geometryLibs)
            {
                XmlNodeList geometryElements = geometryLib.SelectNodes("colladans:geometry", nsmgr);

                foreach (XmlNode geometryElement in geometryElements)
                {
                    if (geometries == null) geometries = new List<Geometry>();
                    geometries.Add(new Geometry(this, geometryElement));
                }
            }

            XmlNodeList controllerLibs = root.SelectNodes("colladans:library_controllers", nsmgr);
            
            foreach (XmlNode controllerLib in controllerLibs)
            {
                XmlNodeList controllerElements = controllerLib.SelectNodes("colladans:controller", nsmgr);

                foreach (XmlNode controllerElement in controllerElements)
                {
                    if (controllers == null) controllers = new List<Controller>();
                    controllers.Add(new Controller(this, controllerElement));
                }
            }

            XmlNodeList nodeLibs = root.SelectNodes("colladans:library_nodes", nsmgr);

            foreach (XmlNode nodeLib in nodeLibs)
            {
                XmlNodeList nodeElements = nodeLib.SelectNodes("colladans:node", nsmgr);

                foreach (XmlNode nodeElement in nodeElements)
                {
                    if (nodes == null) nodes = new List<Node>();
                    nodes.Add(new Node(this, nodeElement));
                }
            }

            XmlNodeList visualSceneLibs = root.SelectNodes("colladans:library_visual_scenes", nsmgr);
            
            foreach (XmlNode visual_sceneLib in visualSceneLibs)
            {
                XmlNodeList visualSceneElements = visual_sceneLib.SelectNodes("colladans:visual_scene", nsmgr);

                foreach (XmlNode visualSceneElement in visualSceneElements)
                {
                    if (visualScenes == null) visualScenes = new List<VisualScene>();
                    visualScenes.Add(new VisualScene(this, visualSceneElement));
                }
            }
            // Load the scene for display in the viewer

            XmlNode sceneElement = root.SelectSingleNode("colladans:scene", nsmgr);
            if (sceneElement != null)
            {
                foreach (XmlNode child in sceneElement.ChildNodes)
                {
                    switch (child.Name)
                    {
                        case "instance_visual_scene":
                            instanceVisualScene = new InstanceScene(this, child);
                            break;
                        case "instance_physics_scene":
                            instancePhysicsScene = new InstanceScene(this, child);
                            break;
                        default:
                            throw new Exception("un-expected <" + child.Name + "> in <scene> :" + filename);
                    }
                }
            }

			XmlNodeList animationsLibs = root.SelectNodes("colladans:library_animations", nsmgr);
			foreach (XmlNode animationsLib in animationsLibs)
			{
				XmlNodeList animationElements = animationsLib.SelectNodes("colladans:animation", nsmgr);
				foreach (XmlNode animationElement in animationElements)
				{
					if (animations == null)
					{
						animations = new List<Animation>();
					}
					animations.Add(new Animation(this, animationElement));
				}
			}
            // release Xml document now that we have COLLADA in memmory
            root = null;
            colladaDocument = null;
        } 
    }
 }
