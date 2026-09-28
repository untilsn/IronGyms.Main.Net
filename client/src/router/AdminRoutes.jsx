import { Route } from "react-router-dom";
import PublicOnlyRoute from "./PublicOnlyRoute";

export default function AdminRoutes() {
  return (
    <Route element={<PublicOnlyRoute area="admin" />}>
      <Route path="/admin" element={<div>AdminRoutes</div>} />
    </Route>
  );
}
