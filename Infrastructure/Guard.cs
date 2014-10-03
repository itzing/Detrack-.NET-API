using System;
using System.Globalization;

namespace Detrack.Infrastructure
{
	/// <summary>
	/// Helper class for guard statements, that allow prettier code for guard clauses
	/// </summary>
	/// <example>
	/// Sample usage:
	/// <code>
	/// <![CDATA[
	/// Guard.Against(name.Length == 0).With<ArgumentException>("Name must have at least 1 char length");
	/// Guard.AgainstNull(obj, "obj");
	/// Guard.AgainstNullOrEmpty(name, "name", "Name must have a value");
	/// ]]>
	/// </code>
	/// </example>
	public static class Guard
	{
		/// <summary>
		/// Checks the supplied condition and act with exception if condition resolves to <c>true</c>.
		/// </summary>
		public static Act Against(bool assertion)
		{
			return new Act(assertion);
		}

		/// <summary>
		/// Checks the value of the supplied <paramref name="value"/> and throws an
		/// <see cref="System.ArgumentNullException"/> if it is <see langword="null"/>.
		/// </summary>
		/// <param name="value">The object to check.</param>
		/// <param name="variableName">The variable name.</param>
		/// <exception cref="System.ArgumentNullException">
		/// If the supplied <paramref name="value"/> is <see langword="null"/>.
		/// </exception>
		public static void AgainstNull<T>(T value, string variableName)
		{
			AgainstNull(value, variableName, string.Format(CultureInfo.InvariantCulture, "'{0}' cannot be null.", variableName));
		}

		/// <summary>
		/// Checks the value of the supplied <paramref name="value"/> and throws an
		/// <see cref="System.ArgumentNullException"/> if it is <see langword="null"/>.
		/// </summary>
		/// <param name="value">The object to check.</param>
		/// <param name="variableName">The variable name.</param>
		/// <param name="message">The message to include in exception description</param>
		/// <exception cref="System.ArgumentNullException">
		/// If the supplied <paramref name="value"/> is <see langword="null"/>.
		/// </exception>
		public static void AgainstNull<T>(T value, string variableName, string message)
		{
			if (value == null)
				throw new ArgumentNullException(variableName, message);
		}

		/// <summary>
		/// Checks the value of the supplied string <paramref name="value"/> and throws an
		/// <see cref="System.ArgumentException"/> if it is <see langword="null"/> or contains only whitespace character(s).
		/// </summary>
		/// <param name="value">The string value to check.</param>
		/// <param name="variableName">The variable name.</param>
		/// <exception cref="System.ArgumentException">
		/// If the supplied <paramref name="value"/> is <see langword="null"/> or contains only whitespace character(s).
		/// </exception>
		public static void AgainstNullOrEmpty(string value, string variableName)
		{
			AgainstNullOrEmpty(value, variableName, TrimOptions.DoTrim);
		}

		/// <summary>
		/// Checks the value of the supplied string <paramref name="value"/> and throws an
		/// <see cref="System.ArgumentException"/> if it is <see langword="null"/> or empty.
		/// </summary>
		/// <param name="value">The string value to check.</param>
		/// <param name="variableName">The argument name.</param>
		/// <param name="options">The value trimming options.</param>
		/// <exception cref="System.ArgumentException">
		/// If the supplied <paramref name="value"/> is <see langword="null"/> or empty.
		/// </exception>
		public static void AgainstNullOrEmpty(string value, string variableName, TrimOptions options)
		{
			string message = string.Format(
				CultureInfo.InvariantCulture,
				"'{0}' cannot be null or resolve to an empty string : '{1}'.", variableName, value);

			AgainstNullOrEmpty(value, variableName, message, options);
		}

		/// <summary>
		/// Checks the value of the supplied string <paramref name="value"/> and throws an
		/// <see cref="System.ArgumentException"/> if it is <see langword="null"/> or empty.
		/// </summary>
		/// <param name="value">The string value to check.</param>
		/// <param name="variableName">The variable name.</param>
		/// <param name="message">The message to include in exception description</param>
		/// <param name="options">The value trimming options.</param>
		/// <exception cref="System.ArgumentException">
		/// If the supplied <paramref name="value"/> is <see langword="null"/> or empty.
		/// </exception>
		public static void AgainstNullOrEmpty(string value, string variableName, string message, TrimOptions options)
		{
			if (value != null && options == TrimOptions.DoTrim)
				value = value.Trim();

			if (string.IsNullOrEmpty(value))
				throw new ArgumentException(message, variableName);
		}
	}

	/// <summary>
	/// Represents action taken when assertion is true
	/// </summary>
	public class Act
	{
		readonly bool assertion;

		internal Act(bool assertion)
		{
			this.assertion = assertion;
		}

		/// <summary>
		/// Will throw an exception of type <typeparamref name="TException"/>
		/// with the specified message if the "Against" assertion is true
		/// </summary>
		/// <typeparam name="TException">Exception type</typeparam>
		/// <param name="message">Exception message</param>
		public void With<TException>(string message) where TException : Exception
		{
			if (assertion)
				throw (TException)Activator.CreateInstance(typeof(TException), message);
		}
	}

	public enum TrimOptions
	{
		DoTrim = 0,
		NoTrim = 1
	}
}