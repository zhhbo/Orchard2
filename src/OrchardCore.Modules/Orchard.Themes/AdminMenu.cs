﻿using Microsoft.Extensions.Localization;
using Orchard.Environment.Navigation;
using System;
using Orchard.DisplayManagement.Theming;
using System.Collections.Generic;
namespace Orchard.Themes
{
    public class AdminMenu : INavigationProvider
    {
        public AdminMenu(IStringLocalizer<AdminMenu> localizer,IEnumerable<IThemeSelector> selector)
        {
            _selector = selector;
            T = localizer;
        }
        private readonly IEnumerable<IThemeSelector> _selector;
        public IStringLocalizer T { get; set; }

        public void BuildNavigation(string name, NavigationBuilder builder)
        {
            if (!String.Equals(name, "admin", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            builder
                .Add(T["Themes"], "10", themes => themes
                    .AddClass("themes").Id("themes")
                    .Permission(Permissions.ApplyTheme)
                    .Add(T["Active Themes"], "0", installed => installed
                        .Action("Index", "Admin", new { area = "Orchard.Themes" })
                        .Permission(Permissions.ApplyTheme)
                        .LocalNav()
                    )
                );
        }
    }
}
