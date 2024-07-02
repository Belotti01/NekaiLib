using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Nekai.Common;

public static class HttpClientExtensions
{
	/// <summary>
	/// Send an HTTP request to the <paramref name="requestUri"/> using the specified <paramref name="method"/>, with the <paramref name="obj"/>
	/// serialized to JSON as content.
	/// </summary>
	/// <param name="method"> The HTTP method of the request. </param>
	/// <param name="requestUri"> The endpoint to send the request to. </param>
	/// <param name="obj"> The content of the request. </param>
	/// <returns> A task resulting in the generated <see cref="HttpResponseMessage"/>. </returns>
	public static async Task<HttpResponseMessage> SendJsonAsync<T>(this HttpClient client, HttpMethod method, string? requestUri, T obj)
	{
		var content = JsonContent.Create(obj);
		return await client.SendAsync(method, requestUri, content);
	}

	/// <summary>
	/// Send an HTTP request to the <paramref name="requestUri"/> using the specified <paramref name="method"/>, optionally containing <paramref name="content"/>.
	/// </summary>
	/// <param name="method"> The HTTP method of the request. </param>
	/// <param name="requestUri"> The endpoint to send the request to. </param>
	/// <param name="content"> The content of the request. </param>
	/// <returns> A task resulting in the generated <see cref="HttpResponseMessage"/>. </returns>
	public static async Task<HttpResponseMessage> SendAsync(this HttpClient client, HttpMethod method, string? requestUri, HttpContent? content = null)
	{
		HttpRequestMessage request = new(method, requestUri)
		{
			Content = content
		};

		return await client.SendAsync(request);
	}
}
