
namespace GMap.NET
{
   using System;
   using System.Collections.Generic;
   using System.Runtime.Serialization;
   using GMap.NET.MapProviders;

   /// <summary>
   /// represents route of map
   /// </summary>
   [Serializable]
#if !PocketPC
   public class MapRoute : ISerializable, IDeserializationCallback
#else
   public class MapRoute
#endif
   {
      /// <summary>
      /// points of route
      /// </summary>
      public readonly List<PointLatLng> Points = new List<PointLatLng>();

      /// <summary>
      /// route info
      /// </summary>
      public string Name;

      /// <summary>
      /// custom object
      /// </summary>
      public object Tag;

      /// <summary>
      /// route start point
      /// </summary>
      public PointLatLng? From
      {
         get
         {
            if(Points.Count > 0)
            {
               return Points[0];
            }

            return null;
         }
      }

      /// <summary>
      /// route end point
      /// </summary>
      public PointLatLng? To
      {
         get
         {
            if(Points.Count > 1)
            {
               return Points[Points.Count - 1];
            }

            return null;
         }
      }

      public MapRoute(string name)
      {
         Name = name;
      }

      public MapRoute(IEnumerable<PointLatLng> points, string name)
      {
         Points.AddRange(points);
         Name = name;
      }

      /// <summary>
      /// route distance (in km)
      /// </summary>
      public double Distance
      {
         get
         {
            double distance = 0.0;

            if(From.HasValue && To.HasValue)
            {
               for(int i = 1; i < Points.Count; i++)
               {
                  distance += GMapProviders.EmptyProvider.Projection.GetDistance(Points[i - 1], Points[i]);
               }
            }

            return distance;
         }
      }    

      /// <summary>
      /// clears points and sets tag and name to null
      /// </summary>
      public void Clear()
      {
         Points.Clear();
         Tag = null;
         Name = null;
      }

#if !PocketPC
      #region ISerializable Members

      // Temp store for de-serialization.
      private PointLatLng[] deserializedPoints;

      /// <summary>
      /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
      /// </summary>
      /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
      /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
      /// <exception cref="T:System.Security.SecurityException">
      /// The caller does not have the required permission.
      /// </exception>
      public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
      {
         info.AddValue("Name", this.Name);
         info.AddValue("Tag", this.Tag);
         info.AddValue("Points", this.Points.ToArray());
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="MapRoute"/> class.
      /// </summary>
      /// <param name="info">The info.</param>
      /// <param name="context">The context.</param>
      protected MapRoute(SerializationInfo info, StreamingContext context)
      {
         this.Name = info.GetString("Name");
         this.Tag = Extensions.GetValue<object>(info, "Tag", null);
         this.deserializedPoints = Extensions.GetValue<PointLatLng[]>(info, "Points");
         this.Points = new List<PointLatLng>();
      }

      #endregion

      #region IDeserializationCallback Members

      /// <summary>
      /// Runs when the entire object graph has been de-serialized.
      /// </summary>
      /// <param name="sender">The object that initiated the callback. The functionality for this parameter is not currently implemented.</param>
      public virtual void OnDeserialization(object sender)
      {
         // Accounts for the de-serialization being breadth first rather than depth first.
         Points.AddRange(deserializedPoints);
         Points.TrimExcess();
      }

      #endregion
#endif
   }
}
