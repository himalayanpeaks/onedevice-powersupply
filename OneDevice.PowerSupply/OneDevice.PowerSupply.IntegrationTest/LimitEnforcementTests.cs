using OneDevice.PowerSupply.Factory;
using static OneDevice.Module.Definition;

namespace OneDevice.PowerSupply.IntegrationTest;

/// <summary>
/// SetVolts/SetAmps are called directly by the gRPC service and must enforce the same limits
/// as the DesiredVolts/DesiredAmps properties.
/// </summary>
public class LimitEnforcementTests
{
    [Theory]
    [InlineData(-0.1)]
    [InlineData(30.1)]
    [InlineData(1000)]
    public void SetVolts_OutsideLimits_IsRejected(double volts)
    {
        var device = MockDeviceHelper.CreateMockedDevice(maxVolts: 30.0);
        Assert.Equal((int)DeviceError.InvalidParameter, device.SetVolts(0, volts));
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(5.1)]
    public void SetAmps_OutsideLimits_IsRejected(double amps)
    {
        var device = MockDeviceHelper.CreateMockedDevice(maxAmps: 5.0);
        Assert.Equal((int)DeviceError.InvalidParameter, device.SetAmps(0, amps));
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(12.5)]
    [InlineData(30.0)]
    public void SetVolts_WithinLimits_IsAccepted(double volts)
    {
        var device = MockDeviceHelper.CreateMockedDevice(maxVolts: 30.0);
        Assert.Equal((int)DeviceError.NoError, device.SetVolts(0, volts));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    public void SetVoltsAndAmps_OnUnknownChannel_AreRejected(int channel)
    {
        var device = MockDeviceHelper.CreateMockedDevice(channelCount: 1);
        Assert.Equal((int)DeviceError.InvalidParameter, device.SetVolts(channel, 1.0));
        Assert.Equal((int)DeviceError.InvalidParameter, device.SetAmps(channel, 1.0));
    }

    [Fact]
    public void Factory_NamesKd3005pDeviceAfterProduct()
    {
        var device = PowerSupplyFactory.Create(PowerSupplyType.Kd3005p);
        Assert.NotNull(device);
        Assert.Equal("Kd3005p", device!.Parameters.Name);
    }
}
