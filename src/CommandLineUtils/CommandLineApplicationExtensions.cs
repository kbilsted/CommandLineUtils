// Copyright (c) Nate McMaster.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// This file has been modified from the original form. See Notice.txt in the project root for more information.

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace McMaster.Extensions.CommandLineUtils
{
    /// <summary> Helper methods for <see cref="CommandLineApplication"/>. </summary>
    public static class CommandLineApplicationExtensions
    {
        /// <summary> Adds a command line argument with values that should be parsable into <typeparamref name="T" />. </summary>
        public static CommandArgument<T> Argument<T>(this CommandLineApplication app, string name, string description, bool multipleValues = false)
            => app.Argument<T>(name, description, _ => { }, multipleValues);

        /// <summary> Adds a command-line option with values that should be parsable into <typeparamref name="T" />. </summary>
        public static CommandOption<T> Option<T>(this CommandLineApplication app, string template, string description, CommandOptionType optionType)
            => app.Option<T>(template, description, optionType, _ => { }, inherited: false);

        /// <summary> Adds a command-line option with values that should be parsable into <typeparamref name="T" />. </summary>
        public static CommandOption<T> Option<T>(this CommandLineApplication app, string template, string description, CommandOptionType optionType, bool inherited)
            => app.Option<T>(template, description, optionType, _ => { }, inherited);

        /// <summary> Adds a command-line option with values that should be parsable into <typeparamref name="T" />. </summary>
        public static CommandOption<T> Option<T>(this CommandLineApplication app, string template, string description, CommandOptionType optionType, Action<CommandOption> configuration)
            => app.Option<T>(template, description, optionType, configuration, inherited: false);

        /// <summary> Adds the help option with the template <c>-?|-h|--help</c>. </summary>
        public static CommandOption HelpOption(this CommandLineApplication app)
            => app.HelpOption(Strings.DefaultHelpTemplate);

        /// <summary> Adds the help option with the template <c>-?|-h|--help</c>. </summary>
        public static CommandOption HelpOption(this CommandLineApplication app, bool inherited)
            => app.HelpOption(Strings.DefaultHelpTemplate, inherited);

        /// <summary> Adds the verbose option with the template <c>-v|--verbose</c>. </summary>
        public static CommandOption VerboseOption(this CommandLineApplication app)
            => VerboseOption(app, "-v|--verbose");

        /// <summary> Adds the verbose option with the template <c>-v|--verbose</c>. </summary>
        /// <param name="template" />
        public static CommandOption VerboseOption(this CommandLineApplication app, string template)
            => app.Option(template, "Show verbose output", CommandOptionType.NoValue, inherited: true);

        /// <summary>
        /// <para>
        /// This method is obsolete and will be removed in a future version.
        /// The recommended alternative is <see cref="OnExecuteAsync" />.
        /// See https://github.com/natemcmaster/CommandLineUtils/issues/275 for details.
        /// </para>
        /// <para>
        /// Sets an async handler with a return code of <c>0</c>.
        /// </para>
        /// </summary>
        /// <param name="action">An asynchronous action to invoke when the ocmmand is selected..</param>
        [Obsolete("This method is obsolete and will be removed in a future version. " +
                  "The recommended replacement is .OnExecuteAsync(). " +
                  "See https://github.com/natemcmaster/CommandLineUtils/issues/275 for details.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void OnExecute(this CommandLineApplication app, Func<Task> action)
        {
            app.OnExecute(async () =>
                       {
                           await action();
                           return 0;
                       });
        }

        /// <summary> Sets an async handler with a return code of <c>0</c>. </summary>
        /// <param name="action">An asynchronous action to invoke when the ocmmand is selected..</param>
        public static void OnExecuteAsync(this CommandLineApplication app, Func<CancellationToken, Task> action)
            => app.OnExecuteAsync(async ct =>
            {
                await action(ct);
                return 0;
            });

        /// <summary> Sets <see cref="CommandLineApplication.Invoke"/> with a return code of <c>0</c>. </summary>
        /// <param name="action">An action to invoke when the command is selected.</param>
        public static void OnExecute(this CommandLineApplication app, Action action)
            => app.OnExecute(() =>
                {
                    action();
                    return 0;
                });

        /// <summary> Sets an action to invoke, but only when there is a validation error. </summary>
        public static void OnValidationError(this CommandLineApplication app, Func<ValidationResult, int> action)
            => app.ValidationErrorHandler = action;

        /// <summary> Sets an action to invoke, but only when there is a validation error. </summary>
        public static void OnValidationError(this CommandLineApplication app, Action<ValidationResult> action)
        {
            app.OnValidationError(r =>
            {
                action(r);
                return 1;
            });
        }

        /// <summary>
        /// Finds <see cref="AssemblyInformationalVersionAttribute"/> on <paramref name="assembly"/> and uses that
        /// to set <see cref="CommandLineApplication.OptionVersion"/>.
        /// <para>
        /// Uses the Version that is part of the <see cref="AssemblyName"/> of the specified assembly
        /// if no <see cref="AssemblyInformationalVersionAttribute"/> is applied.
        /// </para>
        /// </summary>
        /// <exception cref="ArgumentNullException">Either <paramref name="app"/> or <paramref name="assembly"/> is <c>null</c>.</exception>
        public static CommandOption VersionOptionFromAssemblyAttributes(this CommandLineApplication app, Assembly assembly)
            => VersionOptionFromAssemblyAttributes(app, Strings.DefaultVersionTemplate, assembly);

        /// <summary>
        /// Finds <see cref="AssemblyInformationalVersionAttribute"/> on <paramref name="assembly"/> and uses that
        /// to set <see cref="CommandLineApplication.OptionVersion"/>.
        /// <para>
        /// Uses the Version that is part of the <see cref="AssemblyName"/> of the specified assembly
        /// if no <see cref="AssemblyInformationalVersionAttribute"/> is applied.
        /// </para>
        /// </summary>
        /// <exception cref="ArgumentNullException">Either <paramref name="app"/> or <paramref name="assembly"/> is <c>null</c>.</exception>
        public static CommandOption VersionOptionFromAssemblyAttributes(CommandLineApplication app, string template, Assembly assembly)
            => app.VersionOption(template, GetInformationalVersion(assembly));

        private static string? GetInformationalVersion(Assembly assembly)
        {
            var infoVersion = assembly
                ?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion;
            return string.IsNullOrWhiteSpace(infoVersion)
                ? assembly?.GetName().Version.ToString()
                : infoVersion;
        }
    }
}
