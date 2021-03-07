using Unity.Barracuda;

public class ReversiAI
{   
    readonly IWorker worker;

    public ReversiAI(NNModel modelAsset)
    {
        var model = ModelLoader.Load(modelAsset);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.CSharpBurst, model);
    }

    public float[] Inference(float[] inputFloats)
    {
        var inputTensor = new Tensor(1, 1, 1, 64, inputFloats);

        worker.Execute(inputTensor);
        var outputTensor = worker.PeekOutput();
        var outputArray = outputTensor.ToReadOnlyArray();
        
        inputTensor.Dispose();
        outputTensor.Dispose();

        return outputArray;
    }

    ~ReversiAI()
    {
        worker?.Dispose();
    }
}