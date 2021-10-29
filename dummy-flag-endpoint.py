
import asyncio
import random


responses = [
    b"OK",
    b"INV",
    b"OWN",
    b"OLD",
    b"DUP",
]

async def handle_flagsubmission(rx: asyncio.StreamReader, tx: asyncio.StreamWriter) -> None:
    try:
        tx.write(b"This is a dummy flag submission endpoint returning random responses!\nFor dev use only!\n\n")
        await tx.drain()
        while True:
            line = await rx.readline()
            if line == b"":
                break
            
            if line[-1:] != b"\n":
                break
            
            result_string = random.choice(responses)
            tx.write(line[:-1] + b" " + result_string + b"\n")
            await tx.drain()
    except Exception as e:
        print(e) 


async def main():
    server = await asyncio.start_server(handle_flagsubmission, "127.0.0.1", 1337)
    await server.serve_forever()

asyncio.run(main())