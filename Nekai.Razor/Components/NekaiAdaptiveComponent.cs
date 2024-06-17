using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nekai.Razor
{
    public class NekaiAdaptiveComponent : NekaiComponentBase
    {
        /// <inheritdoc cref="FluentGridItem.xxl" />
        public int? Xxl { get; set; }
        /// <inheritdoc cref="FluentGridItem.xl" />
		public int? Xl { get; set; }
        /// <inheritdoc cref="FluentGridItem.lg" />
		public int? Lg { get; set; }
        /// <inheritdoc cref="FluentGridItem.md" />
		public int? Md { get; set; }
        /// <inheritdoc cref="FluentGridItem.sm" />
		public int? Sm { get; set; }
        /// <inheritdoc cref="FluentGridItem.xs" />
		public int? Xs { get; set; }
	}
}
