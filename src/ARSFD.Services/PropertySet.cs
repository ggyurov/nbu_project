using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ARSFD.Services
{
	/// <summary>
	/// Represents a property set.
	/// </summary>
	public abstract class PropertySet
	{
		protected Dictionary<string, object> Properties { get; } = new Dictionary<string, object>();

		/// <summary>
		/// Tries to get a value of a property.
		/// </summary>
		/// <typeparam name="TValue">type of value</typeparam>
		/// <param name="value">property value</param>
		/// <param name="name">name</param>
		/// <returns><value>true</value> if the value is present, otherwise <value>false</value></returns>
		public bool TryGet<TValue>(out TValue value, [CallerMemberName] string name = null)
		{
			if (!Properties.TryGetValue(name, out object obj))
			{
				value = default;
				return false;
			}

			value = (TValue)obj;

			return true;
		}

		/// <summary>
		/// Gets a value of a property.
		/// </summary>
		/// <typeparam name="TValue">type of value</typeparam>
		/// <param name="name">name</param>
		/// <returns>property value</returns>
		public TValue Get<TValue>([CallerMemberName] string name = null)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (!TryGet(out TValue value, name))
			{
				throw new IndexOutOfRangeException(string.Format("Value for property `{0}` is not set.", name));
			}

			return value;
		}

		/// <summary>
		/// Sets a value to a property.
		/// </summary>
		/// <typeparam name="TValue">type of value</typeparam>
		/// <param name="name">name</param>
		/// <param name="value">value</param>
		public void Set<TValue>(TValue value, [CallerMemberName] string name = null)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			lock (Properties)
			{
				Properties[name] = value;
			}
		}

		/// <summary>
		/// Unsets a value of a property.
		/// </summary>
		/// <typeparam name="TValue">type of value</typeparam>
		/// <param name="name">name</param>
		public void Unset<TValue>([CallerMemberName] string name = null)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			lock (Properties)
			{
				if (Properties.ContainsKey(name))
				{
					Properties.Remove(name);
				}
			}
		}

		/// <summary>
		/// Checks whether a property value is set or not.
		/// </summary>
		/// <typeparam name="TValue">type of value</typeparam>
		/// <param name="name">name</param>
		/// <returns><value>true</value> if the value is set, otherwise - <value>false</value></returns>
		public bool IsSet<TValue>([CallerMemberName] string name = null)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			return Properties.ContainsKey(name);
		}
	}
}
