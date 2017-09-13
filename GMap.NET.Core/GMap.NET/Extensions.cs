
namespace GMap.NET
{
   using System;
   using System.Runtime.Serialization;
   using System.Diagnostics;

   public static class Extensions
   {
      /// <summary>
      /// Retrieves a value from the SerializationInfo of the given type.
      /// </summary>
      /// <typeparam name="T">The Type that we are attempting to de-serialize.</typeparam>
      /// <param name="info">The SerializationInfo.</param>
      /// <param name="key">The key of the value we wish to retrieve.</param>
      /// <returns>The value if found, otherwise null.</returns>
      public static T GetValue<T>(SerializationInfo info, string key) where T : class
      {
         try
         {
            // Return the value from the SerializationInfo, casting it to type T.
            return info.GetValue(key, typeof(T)) as T;
         }
         catch(Exception ex)
         {
            Debug.WriteLine("Extensions.GetValue: " + ex.Message);
            return null;
         }
      }

      /// <summary>
      /// Retrieves a value from the SerializationInfo of the given type.
      /// </summary>
      /// <typeparam name="T">The Type that we are attempting to de-serialize.</typeparam>
      /// <param name="info">The SerializationInfo.</param>
      /// <param name="key">The key of the value we wish to retrieve.</param>
      /// <param name="defaultValue">The default value if the de-serialized value was null.</param>
      /// <returns>The value if found, otherwise the default value.</returns>
      public static T GetValue<T>(SerializationInfo info, string key, T defaultValue) where T : class
      {
         T deserializedValue = GetValue<T>(info, key);
         if(deserializedValue != null)
         {
            return deserializedValue;
         }

         return defaultValue;
      }

      /// <summary>
      /// Retrieves a value from the SerializationInfo of the given type for structs.
      /// </summary>
      /// <typeparam name="T">The Type that we are attempting to de-serialize.</typeparam>
      /// <param name="info">The SerializationInfo.</param>
      /// <param name="key">The key of the value we wish to retrieve.</param>
      /// <param name="defaultValue">The default value if the de-serialized value was null.</param>
      /// <returns>The value if found, otherwise the default value.</returns>
      public static T GetStruct<T>(SerializationInfo info, string key, T defaultValue) where T : struct
      {
         try
         {
            return (T)info.GetValue(key, typeof(T));
         }
         catch(Exception ex)
         {
            Debug.WriteLine("Extensions.GetStruct: " + ex.Message);
            return defaultValue;
         }
      }

      /// <summary>
      /// Retrieves a value from the SerializationInfo of the given type for structs.
      /// </summary>
      /// <typeparam name="T">The Type that we are attempting to de-serialize.</typeparam>
      /// <param name="info">The SerializationInfo.</param>
      /// <param name="key">The key of the value we wish to retrieve.</param>
      /// <param name="defaultValue">The default value if the de-serialized value was null.</param>
      /// <returns>The value if found, otherwise the default value.</returns>
      public static Nullable<T> GetStruct<T>(SerializationInfo info, string key, Nullable<T> defaultValue) where T : struct
      {
         try
         {
            return (Nullable<T>)info.GetValue(key, typeof(Nullable<T>));
         }
         catch(Exception ex)
         {
            Debug.WriteLine("Extensions.GetStruct: " + ex.Message);
            return defaultValue;
         }
      }
   }
}