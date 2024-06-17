using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Nekai.Razor
{
    public class NekaiAdaptiveComponent : NekaiComponentBase
    {
		/// <inheritdoc cref="FluentGridItem.xxl" />
        [Parameter]
		public int? Xxl { get; set; }
        /// <inheritdoc cref="FluentGridItem.xl" />
        [Parameter]
		public int? Xl { get; set; }
        /// <inheritdoc cref="FluentGridItem.lg" />
        [Parameter]
		public int? Lg { get; set; }
        /// <inheritdoc cref="FluentGridItem.md" />
        [Parameter]
		public int? Md { get; set; }
        /// <inheritdoc cref="FluentGridItem.sm" />
        [Parameter]
		public int? Sm { get; set; }
        /// <inheritdoc cref="FluentGridItem.xs" />
        [Parameter]
		public int? Xs { get; set; }
	}
}
