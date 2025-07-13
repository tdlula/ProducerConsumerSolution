# ProducerConsumerSolution.UI

## Overview

ProducerConsumerSolution.UI is a Windows Forms (.NET 9) application that demonstrates the Producer-Consumer pattern with real-time statistics, queue visualization, and logging. The UI allows users to configure producer and consumer delays, set the number of consumers, and observe the system's behavior as items are produced and consumed.

## Features

- **Start/Stop Controls:** Easily start or stop the producer-consumer system.
- **Configuration:** Set producer delay, consumer delay, and consumer count.
- **Statistics:** View live statistics for queue size, total produced, total consumed, and elapsed time.
- **Queue Display:** See the current items in the queue and their producers.
- **Log Output:** All system events, warnings, and errors are logged with timestamps and color-coded by severity.
- **Responsive UI:** Controls are enabled/disabled based on system state for a smooth user experience.

## How It Works

- Click **Start** to begin producing and consuming items according to your configuration.
- The **Statistics** section updates in real time as items are produced and consumed.
- The **Queue** section shows the most recent items in the queue.
- The **Log Output** section displays all system messages, including errors and status updates.
- Click **Stop** to halt the system; controls become re-enabled for further configuration.
- Click **Clear Log** to clear the log output area.

## How to Run

1. **Prerequisites:**
   - [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
   - Windows OS (uses Windows Forms)

2. **Build the Solution:**
   - Open the solution in Visual Studio 2022 or later.
   - Restore NuGet packages if prompted.
   - Build the solution (`Ctrl+Shift+B`).

3. **Run the Application:**
   - Set `ProducerConsumerSolution.UI` as the startup project.
   - Press `F5` or click **Start** in Visual Studio.

4. **Usage:**
   - Click **Start** to begin.
   - Observe statistics, queue, and log output.
   - Click **Stop** to halt the system.
   - Click **Clear Log** to clear the log area.

## Project Structure

- **MainForm.cs:** The main UI logic and event handling.
- **Domain Interfaces/Models:** Abstract the producer, consumer, queue, and logging logic.
- **Infrastructure/Services:** (Referenced projects) Implement the actual producer-consumer and logging mechanisms.



