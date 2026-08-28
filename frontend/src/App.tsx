import { BrowserRouter, Route, Routes } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { Layout } from './components/Layout';
import { Dashboard } from './pages/Dashboard';
import { History } from './pages/History';
import { GuildProfile } from './pages/GuildProfile';
import { BossDetail } from './pages/BossDetail';
import { SubmitKill } from './pages/SubmitKill';

const queryClient = new QueryClient();

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <Routes>
          <Route element={<Layout />}>
            <Route index element={<Dashboard />} />
            <Route path="history" element={<History />} />
            <Route path="guilds/:id" element={<GuildProfile />} />
            <Route path="bosses/:id" element={<BossDetail />} />
            <Route path="submit" element={<SubmitKill />} />
          </Route>
        </Routes>
      </BrowserRouter>
    </QueryClientProvider>
  );
}

export default App;
