using Microsoft.AspNetCore.Mvc;
using NuGet.Next.Core;
using NuGet.Next.Filter;
using NuGet.Next.Options;
using NuGet.Next.Protocol.Models;

namespace NuGet.Next.Service;

public static class ApiExtensions
{
    public static IEndpointRouteBuilder MapApis(this IEndpointRouteBuilder app)
    {
        var options = app.ServiceProvider.GetRequiredService<NuGetNextOptions>();

        var group = app.MapGroup(options.PathBase ?? "/")
            .AddEndpointFilter<ExceptionFilter>();


        group.Map("/v3/package/{id}/index.json",
                async ([FromServices] PackageApis apis, HttpContext context, string id) =>
                await apis.GetPackageVersionsAsync(context, id))
            .WithOpenApi();

        group.Map("v3/package/{id}/{version}/{idVersion}.nupkg",
                async ([FromServices] PackageApis apis, HttpContext context, string id, string version) =>
                await apis.DownloadPackageAsync(context, id, version))
            .WithOpenApi();

        group.Map("v3/package/{id}/{version}/{id2}.nuspec",
                async ([FromServices] PackageApis apis, HttpContext context, string id, string version) =>
                await apis.DownloadNuspecAsync(context, id, version))
            .WithOpenApi();

        group.Map("v3/package/{id}/{version}/readme",
                async ([FromServices] PackageApis apis, HttpContext context, string id, string version) =>
                await apis.DownloadReadmeAsync(context, id, version))
            .WithOpenApi();

        group.Map("v3/package/{id}/{version}/icon",
                async ([FromServices] PackageApis apis, HttpContext context, string id, string version) =>
                await apis.DownloadIconAsync(context, id, version))
            .WithOpenApi();

        group.MapGet("v3/package/list",
                async ([FromServices] PackageApis apis, HttpContext context, int page, int pageSize,
                        string? keyword, string[] userIds) =>
                    await apis.GetListAsync(page, pageSize, keyword, userIds))
            .WithOpenApi();

        group.MapDelete("v3/package/{id}/{version}",
                async ([FromServices] PackageApis apis, string id, string version) =>
                await apis.DeleteAsync(id, version))
            .WithOpenApi();

        group.Map("/v3/registration/{id}/index.json",
                async ([FromServices] PackageMetadataApis apis, HttpContext context, string id) =>
                await apis.RegistrationIndexAsync(context, id))
            .WithOpenApi();

        group.Map("/v3/registration/{id}/{version}.json",
                async ([FromServices] PackageMetadataApis apis, HttpContext context, string id, string version) =>
                await apis.RegistrationLeafAsync(context, id, version))
            .WithOpenApi();

        group.MapGet("v3/package-info/{id}/{version}",
                async ([FromServices] PackageMetadataApis apis, HttpContext context, string id, string version) =>
                await apis.GetAsync(context, id, version))
            .WithOpenApi();

        group.MapGet("v3/package-info/{id}",
                async ([FromServices] PackageMetadataApis apis, HttpContext context, string id) =>
                await apis.GetAsync(context, id, string.Empty))
            .WithOpenApi();

        group.MapPut("api/v2/package",
                async ([FromServices] PackagePublishApis apis, HttpContext context) =>
                await apis.UploadAsync(context))
            .WithOpenApi();

        group.MapDelete("api/v2/package/{id}/{version}",
                async ([FromServices] PackagePublishApis apis, HttpContext context, string id, string version) =>
                await apis.Delete(context, id, version))
            .WithOpenApi();

        group.MapPost("api/v2/package/{id}/{version}",
                async ([FromServices] PackagePublishApis apis, HttpContext context, string id, string version) =>
                await apis.Delete(context, id, version))
            .WithOpenApi();

        group.Map("v3/search",
                async ([FromServices] SearchApis apis, HttpContext context,
                        [FromQuery(Name = "q")] string? query = null,
                        [FromQuery] int skip = 0,
                        [FromQuery] int take = 20,
                        [FromQuery] bool prerelease = false,
                        [FromQuery] string? semVerLevel = null,
                        [FromQuery] string? packageType = null,
                        [FromQuery] string? framework = null) =>
                    await apis.SearchAsync(context, query, skip, take, prerelease, semVerLevel, packageType, framework))
            .WithOpenApi();

        group.Map("v3/autocomplete",
                async ([FromServices] SearchApis apis, HttpContext context,
                        [FromQuery(Name = "q")] string? autocompleteQuery = null,
                        [FromQuery(Name = "id")] string? versionsQuery = null,
                        [FromQuery] bool prerelease = false,
                        [FromQuery] string? semVerLevel = null,
                        [FromQuery] int skip = 0,
                        [FromQuery] int take = 20,

                        // These are unofficial parameters
                        [FromQuery] string? packageType = null) =>
                    await apis.AutocompleteAsync(autocompleteQuery, versionsQuery, prerelease, semVerLevel, skip, take,
                        packageType, context.RequestAborted))
            .WithOpenApi();

        group.Map("v3/dependents",
                async ([FromServices] SearchApis apis,
                        HttpContext context,
                        [FromQuery] string? packageId = null) =>
                    await apis.DependentsAsync(context, packageId))
            .WithOpenApi();

        group.MapGet("v3/index.json",
                async ([FromServices] ServiceIndexService apis, HttpContext context) =>
                await apis.GetAsync(context.RequestAborted))
            .WithOpenApi();

        group.MapPut("api/v2/symbol",
                async ([FromServices] SymbolApis apis, HttpContext context) =>
                await apis.Upload(context))
            .WithOpenApi();

        group.MapGet("api/download/symbols/{file}/{key}/{file2}",
                async ([FromServices] SymbolApis apis, HttpContext context, string file, string key, string file2) =>
                await apis.Get(context, file, key))
            .WithOpenApi();


        group.MapPost("api/v2/authenticate",
                async ([FromServices] AuthenticationApis apis, AuthenticateInput input) =>
                await apis.AuthenticateAsync(input))
            .WithOpenApi();

        group.MapGet("api/v3/package-update-record",
                async ([FromServices] PackageUpdateRecordApis apis, int page, int pageSize) =>
                await apis.GetCurrentAsync(page, pageSize))
            .WithOpenApi();

        group.MapGet("api/v3/package-update-record/by-user",
                async ([FromServices] PackageUpdateRecordApis apis, string[] userId, int page, int pageSize) =>
                await apis.GetByUserIdAsync(userId, page, pageSize))
            .WithOpenApi();

        var user = group.MapGroup("api/v3/user");

        user.MapPost(string.Empty,
                async ([FromServices] UserApis apis, UserInput input) =>
                await apis.CreateAsync(input))
            .WithOpenApi();

        user.MapGet(string.Empty,
                async ([FromServices] UserApis apis, string? keyword, int page, int pageSize) =>
                await apis.GetAsync(keyword, page, pageSize))
            .WithOpenApi();

        user.MapDelete("{id}",
                async ([FromServices] UserApis apis, string id) =>
                await apis.DeleteAsync(id))
            .WithOpenApi();

        user.MapPut("update-password",
                async ([FromServices] UserApis apis, UpdatePasswordInput input) =>
                await apis.UpdatePasswordAsync(input))
            .WithOpenApi();


        var userKey = group.MapGroup("api/v3/user-key");

        userKey.MapPost(string.Empty,
                async ([FromServices] UserKeyApis apis) =>
                await apis.CreateAsync())
            .WithOpenApi();

        userKey.MapGet(string.Empty,
                async ([FromServices] UserKeyApis apis) =>
                await apis.GetListAsync())
            .WithOpenApi();

        userKey.MapDelete("{id}",
                async ([FromServices] UserKeyApis apis, string id) =>
                await apis.DeleteAsync(id))
            .WithOpenApi();

        userKey.MapPut("enable/{id}",
                async ([FromServices] UserKeyApis apis, string id) =>
                await apis.EnableAsync(id))
            .WithOpenApi();

        var panel = group.MapGroup("api/v3/panel");

        panel.MapGet(string.Empty,
                async ([FromServices] PanelApi apis) =>
                await apis.GetAsync())
            .WithOpenApi();

        return app;
    }
}