import { useState } from 'react';
import { BrowserRouter, Route, Routes } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { Layout } from './components/Layout';
import { SplashScreen } from './components/SplashScreen';
import { Dashboard } from './pages/Dashboard';
import { History } from './pages/History';
import { GuildProfile } from './pages/GuildProfile';
import { BossDetail } from './pages/BossDetail';
import { SubmitKill } from './pages/SubmitKill';
import { PvpLadder } from './pages/PvpLadder';

const queryClient = new QueryClient();

function App() {
  const [showSplash, setShowSplash] = useState(true);

  return (
    <QueryClientProvider client={queryClient}>
      {showSplash && <SplashScreen onDone={() => setShowSplash(false)} />}
      <BrowserRouter>
        <Routes>
          <Route element={<Layout />}>
            <Route index element={<Dashboard />} />
            <Route path="history" element={<History />} />
            <Route path="guilds/:id" element={<GuildProfile />} />
            <Route path="bosses/:id" element={<BossDetail />} />
            <Route path="submit" element={<SubmitKill />} />
            <Route path="pvp" element={<PvpLadder />} />
          </Route>
        </Routes>
      </BrowserRouter>
    </QueryClientProvider>
  );
}

export default App;
