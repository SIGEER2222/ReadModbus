// See https://aka.ms/new-console-template for more information
using EasyModbus;

Console.WriteLine("Hello, World!");

await Set();
await ReadRegistersAsync();
ModbusClient modbusClient;

const int StartAddress = 0;
const int EndAddress = 102;
const int BatchSize = 100;
const int DelayMilliseconds = 5000;
const int GroupSize = 10;

async Task ReadRegistersAsync()
{
    modbusClient = new ModbusClient("192.168.1.18", 31001);
    try
    {
        modbusClient.Connect();
        while (true)
        {
            for (int i = StartAddress; i <= EndAddress; i += BatchSize)
            {
                int currentBatchEnd = i + BatchSize > EndAddress ? EndAddress : i + BatchSize - 1;
                int[] registerValues = modbusClient.ReadHoldingRegisters(i, currentBatchEnd - i + 1);

                for (int j = 0; j < registerValues.Length; j++)
                {
                    if (j % GroupSize == 0)
                    {
                        Console.WriteLine("Address\tValue");
                        Console.WriteLine("-------\t-----");
                    }

                    Console.WriteLine($"{i + j}\t{registerValues[j]}");

                    if ((j + 1) % GroupSize == 0 || j == registerValues.Length - 1)
                    {
                        Console.WriteLine(); // Empty line after a group or the last line
                    }
                }
            }

            // Asynchronous wait
            await Task.Delay(DelayMilliseconds);
        }
    }
    catch (Exception e)
    {
        Console.WriteLine($"Error reading: {e.Message}");
    }
    finally
    {
        // Disconnect
        modbusClient.Disconnect();
    }
}

async Task Set()
{
    ModbusClient modbusClient = new ModbusClient("192.168.1.18", 31001);
    modbusClient.Connect();

    int startAddress = 0;
    int endAddress = 102;
    int batchSize = 100; // 每批读取的寄存器数量

    while (true)
    {
        try
        {
            for (int i = startAddress; i <= endAddress; i += batchSize)
            {
                // 计算当前批次的结束地址
                int currentBatchEnd = i + batchSize > endAddress ? endAddress : i + batchSize;
                // 读取当前批次的寄存器值
                int[] registerValues = modbusClient.ReadHoldingRegisters(i, currentBatchEnd - i + 1);

                // 打印当前批次的寄存器值
                for (int j = 0; j < registerValues.Length; j++)
                {
                    Console.Write($"Register {i + j + 1}: {registerValues[j]}  ");
                }
            }

            // 等待0.5秒
            Thread.Sleep(5000);
        }
        catch (Exception e)
        {
            Console.WriteLine($"读取错误: {e.Message}");
        }
    }

    // 断开连接
    modbusClient.Disconnect();
}
