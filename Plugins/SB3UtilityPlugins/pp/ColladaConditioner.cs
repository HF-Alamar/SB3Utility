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
 */

#region Using Statements
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using COLLADA;

#endregion

namespace COLLADA
{
    public class Conditioner
    {
        /// <summary>This method will convert convex polygons to triangles
        /// <para>A more advanced condionner would be required to handle convex, complex polygons</para>
        /// </summary>
        public static void ConvexTriangulator(Document doc)
        {
            foreach (Document.Geometry geo in doc.geometries)
            {
                List<Document.Primitive> triangles = new List<Document.Primitive>();
                foreach (Document.Primitive primitive in geo.mesh.primitives)
                {
                    if (primitive is Document.Polylist)
                    {
                        int triangleCount = 0;

                        foreach (int vcount in primitive.vcount) triangleCount += vcount - 2;
                        int[] newP = new int[primitive.stride * triangleCount * 3];
                        int count = 0;
                        int offset = 0;
                        int first = 0;
                        int last = 0;
                        int j,k;

                        foreach (int vcount in primitive.vcount)
                        {
                            first = offset;
                            last = first + 1;
                            for (j = 0; j < vcount - 2; j++)
                            {
                                // copy first
                                for (k = 0; k < primitive.stride; k++)
                                    newP[count++] = primitive.p[k + first * primitive.stride];
                                // copy previous last
                                for (k = 0; k < primitive.stride; k++)
                                    newP[count++] = primitive.p[k + last * primitive.stride];
                                last += 1;
                                // last = new point
                                for (k = 0; k < primitive.stride; k++)
                                    newP[count++] = primitive.p[k + last * primitive.stride];
                            }
                            offset = last + 1;
                        }
                        // Make a triangle out of this Polylist
                        Document.Triangle triangle = new Document.Triangle(doc, count / primitive.stride / 3, primitive.Inputs, newP);
                        triangle.name = primitive.name;
                        triangle.material = primitive.material;
                        triangle.extras = primitive.extras;
                        triangles.Add(triangle);
                    }
                    else if (primitive is Document.Triangle) 
                    {
                        triangles.Add(primitive);
                    } else if (primitive is Document.Line)
                    {
                        // remove lines for now...
                    }  else
                        throw new Exception("Unsupported primitive "+primitive.GetType().ToString()+" in Conditioner::ConvexTriangle");
                }
                geo.mesh.primitives = triangles;
            }
        }
    }
}
