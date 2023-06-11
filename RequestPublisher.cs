using System;

namespace ciklonalozi;

public static class RequestPublisher
{
    public static event Action OnReceived = delegate { };
    public static void Raise() => OnReceived();
}