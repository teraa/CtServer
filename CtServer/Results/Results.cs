namespace CtServer.Results;

public record struct Success;
public record struct NotFound;
public record struct Fail(string Error);
