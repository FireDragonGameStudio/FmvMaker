using Unity.VisualScripting;

namespace FmvMaker.Graph {
    [Inspectable]
    public class FmvViewNode {

        [Inspectable]
        private int outputPortCount = 2;

        public void AddIngredientPort(out int test) {
            outputPortCount++;
            test = 0;
            //Definition();
        }

        public void RemoveIngredientPort() {
            outputPortCount--;
            if (outputPortCount < 2)
                outputPortCount = 2;

            //Definition();
        }
    }
}
