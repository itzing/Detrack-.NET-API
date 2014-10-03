using System;
using System.Linq.Expressions;

namespace Detrack.Infrastructure
{
	public static class ReflectionHelper
	{
		public static string GetMemberName<T>(Expression<Func<T>> memberExpression)
		{
			var expressionBody = (MemberExpression)memberExpression.Body;
			return expressionBody.Member.Name;
		}
	}
}
