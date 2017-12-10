using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ARSFD.Services
{
	/// <summary>
	/// Represents a property set.
	/// </summary>
	/// <typeparam name="TPropertySet">type to wrap</typeparam>
	public abstract class PropertySet<TPropertySet> : PropertySet
		where TPropertySet : PropertySet
	{
		/// <summary>
		/// Tries to get a value of a property.
		/// </summary>
		/// <typeparam name="TValue">type of value</typeparam>
		/// <param name="expr">property expression</param>
		/// <param name="value">property value</param>
		/// <returns><value>true</value> if the value is present, otherwise <value>false</value></returns>
		public bool TryGet<TValue>(Expression<Func<TPropertySet, TValue>> expr, out TValue value)
		{
			string propertyName = GetPropertyName(expr);

			return TryGet(out value, propertyName);
		}

		/// <summary>
		/// Gets a value of a property.
		/// </summary>
		/// <typeparam name="TValue">type of value</typeparam>
		/// <param name="expr">property expression</param>
		/// <returns>property value</returns>
		public TValue Get<TValue>(Expression<Func<TPropertySet, TValue>> expr)
		{
			string propertyName = GetPropertyName(expr);

			return Get<TValue>(propertyName);
		}

		/// <summary>
		/// Sets a value to a property.
		/// </summary>
		/// <typeparam name="TValue">type of value</typeparam>
		/// <param name="expr">property expression</param>
		/// <param name="value">value</param>
		public void Set<TValue>(Expression<Func<TPropertySet, TValue>> expr, TValue value)
		{
			string propertyName = GetPropertyName(expr);

			Set(value, propertyName);
		}

		/// <summary>
		/// Unsets a value of a property.
		/// </summary>
		/// <typeparam name="TValue">type of value</typeparam>
		/// <param name="expr">property expression</param>
		public void Unset<TValue>(Expression<Func<TPropertySet, TValue>> expr)
		{
			string propertyName = GetPropertyName(expr);

			Unset<TValue>(propertyName);
		}

		/// <summary>
		/// Checks whether a property value is set or not.
		/// </summary>
		/// <typeparam name="TValue">type of value</typeparam>
		/// <param name="expr">property expression</param>
		/// <returns><value>true</value> if the value is set, otherwise - <value>false</value></returns>
		public bool IsSet<TValue>(Expression<Func<TPropertySet, TValue>> expr)
		{
			string propertyName = GetPropertyName(expr);

			return IsSet<TValue>(propertyName);
		}

		/// <summary>
		/// Gets the metadata of a property.
		/// </summary>
		/// <typeparam name="TValue">type of value</typeparam>
		/// <param name="expr">property expression</param>
		/// <returns>property metadata</returns>
		private static string GetPropertyName<TValue>(Expression<Func<TPropertySet, TValue>> expr)
		{
			if (expr == null)
			{
				throw new ArgumentNullException(nameof(expr));
			}

			var memberExpression = (MemberExpression)expr.Body;
			if (memberExpression.Member.MemberType != MemberTypes.Property)
			{
				throw new ArgumentException("Member is not a property.", nameof(expr));
			}

			var propertyInfo = (PropertyInfo)memberExpression.Member;

			return propertyInfo.Name;
		}
	}
}
