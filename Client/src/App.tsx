import { Provider } from "react-redux";
import { RouterProvider } from "react-router-dom";
import { router } from "./core/router/router";
import { store } from "./core/store";

export default function App() {
  return (
    <Provider store={store}>
      <RouterProvider router={router} />
    </Provider>
  );
}
