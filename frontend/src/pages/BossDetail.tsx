import { useParams } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { fetchBossPulls } from '../lib/api';
import { PullChart } from '../components/PullChart';

export function BossDetail() {
  const { id } = useParams<{ id: string }>();
  const { data: series, isLoading } = useQuery({
    queryKey: ['boss', id, 'pulls'],
    queryFn: () => fetchBossPulls(id!),
    enabled: !!id,
  });

  return (
    <div>
      <p className="eyebrow text-xs text-turquoise">Boss-Detail</p>
      <h1 className="font-headline mb-6 text-4xl">Pull-Verlauf</h1>

      {isLoading && <p className="text-text-muted">Lade Pull-Daten…</p>}
      {series && <PullChart series={series} />}
    </div>
  );
}
